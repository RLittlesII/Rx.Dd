
    public static class DynamicDataExtensions
    {
        public static IObservable<PreviousPropertyValue<TObject, TProperty>> WhenPropertyChanging<TObject, TProperty>(this TObject source,
                                                                                                                      Expression<Func<TObject, TProperty>> propertyAccessor,
                                                                                                                      bool notifyOnInitialValue = true,
                                                                                                                      Func<TProperty> fallbackValue = null)
            where TObject : INotifyPropertyChanging
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (propertyAccessor == null)
            {
                throw new ArgumentNullException(nameof(propertyAccessor));
            }

            var cache = ObservablePropertyChangingFactoryCache.Instance.GetFactory(propertyAccessor);

            return cache.Create(source, notifyOnInitialValue)
                        .Where(pv => !pv.UnobtainableValue || (pv.UnobtainableValue && fallbackValue != null));
        }
    }

    internal sealed class ObservablePropertyChangingFactoryCache
    {
        private readonly ConcurrentDictionary<string, object> _factories = new ConcurrentDictionary<string, object>();

        public static readonly ObservablePropertyChangingFactoryCache Instance = new ObservablePropertyChangingFactoryCache();

        private ObservablePropertyChangingFactoryCache() {}

        public ObservablePropertyFactory<TObject, TProperty> GetFactory<TObject, TProperty>(Expression<Func<TObject, TProperty>> expression)
            where TObject : INotifyPropertyChanging
        {
            var key = expression.ToCacheKey();

            var result = _factories.GetOrAdd(key, k =>
                                                  {
                                                      ObservablePropertyFactory<TObject, TProperty> factory;

                                                      var memberChain = expression.GetMemberChain()
                                                                                  .ToArray();

                                                      if (memberChain.Length == 1)
                                                      {
                                                          factory = new ObservablePropertyFactory<TObject, TProperty>(expression);
                                                      }
                                                      else
                                                      {
                                                          var chain = memberChain.Select(m => new ObservablePropertyPart(m))
                                                                                 .ToArray();

                                                          var accessor = expression?.Compile() ?? throw new ArgumentNullException(nameof(expression));
                                                          factory = new ObservablePropertyFactory<TObject, TProperty>(accessor, chain);
                                                      }

                                                      return factory;
                                                  });

            return (ObservablePropertyFactory<TObject, TProperty>) result;
        }
    }

    internal static class ExpressionBuilder
    {
        internal static string ToCacheKey<TObject, TProperty>(this Expression<Func<TObject, TProperty>> expression)
            where TObject : INotifyPropertyChanging
        {
            var members = expression.GetMembers();

            IEnumerable<string> GetNames()
            {
                yield return typeof(TObject).FullName;

                foreach (var member in members.Reverse())
                {
                    yield return member.Member.Name;
                }
            }

            return string.Join(".", GetNames());
        }

        public static IEnumerable<MemberExpression> GetMembers<TObject, TProperty>(this Expression<Func<TObject, TProperty>> source)
        {
            var memberExpression = source.Body as MemberExpression;

            while (memberExpression != null)
            {
                yield return memberExpression;
                memberExpression = memberExpression.Expression as MemberExpression;
            }
        }

        internal static IEnumerable<MemberExpression> GetMemberChain<TObject, TProperty>(this Expression<Func<TObject, TProperty>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            while (memberExpression != null)
            {
                if (memberExpression.Expression.NodeType != ExpressionType.Parameter)
                {
                    var parent = memberExpression.Expression;

                    yield return memberExpression.Update(Expression.Parameter(parent.Type));
                }
                else
                {
                    yield return memberExpression;
                }

                memberExpression = memberExpression.Expression as MemberExpression;
            }
        }

        internal static Func<object, object> CreateValueAccessor(this MemberExpression source)
        {
            //create an expression which accepts the parent and returns the child
            var property = source.GetProperty();
            var method = property.GetMethod;

            //convert the parameter i.e. the declaring class to an object
            var parameter = Expression.Parameter(typeof(object));
            var converted = Expression.Convert(parameter, source.Expression.Type);

            //call the get value of the property and box it
            var propertyCall = Expression.Call(converted, method);
            var boxed = Expression.Convert(propertyCall, typeof(object));
            var accessorExpr = Expression.Lambda<Func<object, object>>(boxed, parameter);

            var accessor = accessorExpr.Compile();

            return accessor;
        }

        internal static Func<object, IObservable<Unit>> CreatePropertyChangedFactory(this MemberExpression source)
        {
            var property = source.GetProperty();

            var inpc = typeof(INotifyPropertyChanged).GetTypeInfo()
                                                     .IsAssignableFrom(property.DeclaringType.GetTypeInfo());

            return t =>
                   {
                       if (t == null)
                       {
                           return Observable.Never<Unit>();
                       }

                       if (!inpc)
                       {
                           return Observable.Return(Unit.Default);
                       }

                       return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(handler => ((INotifyPropertyChanged) t).PropertyChanged += handler,
                                                                                                                 handler => ((INotifyPropertyChanged) t).PropertyChanged -= handler)
                                        .Where(args => args.EventArgs.PropertyName == property.Name)
                                        .Select(args => Unit.Default);
                   };
        }

        internal static PropertyInfo GetProperty<TObject, TProperty>(this Expression<Func<TObject, TProperty>> expression)
        {
            var property = expression.GetMember() as PropertyInfo;

            if (property == null)
            {
                throw new ArgumentException("Not a property expression");
            }

            return property;
        }

        internal static PropertyInfo GetProperty(this MemberExpression expression)
        {
            var property = expression.Member as PropertyInfo;

            if (property == null)
            {
                throw new ArgumentException("Not a property expression");
            }

            return property;
        }

        internal static MemberInfo GetMember<TObject, TProperty>(this Expression<Func<TObject, TProperty>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentException("Not a property expression");
            }

            return GetMemberInfo(expression);
        }

        private static MemberInfo GetMemberInfo(LambdaExpression lambda)
        {
            if (lambda == null)
            {
                throw new ArgumentException("Not a property expression");
            }

            MemberExpression memberExpression = null;

            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpression = ((UnaryExpression) lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = lambda.Body as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.Call)
            {
                return ((MethodCallExpression) lambda.Body).Method;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("Not a member access");
            }

            return memberExpression.Member;
        }
    }

    internal sealed class ObservablePropertyFactory<TObject, TProperty>
        where TObject : INotifyPropertyChanging
    {
        private readonly Func<TObject, bool, IObservable<PreviousPropertyValue<TObject, TProperty>>> _factory;

        public ObservablePropertyFactory(Func<TObject, TProperty> valueAccessor, ObservablePropertyPart[] chain)
        {
            _factory = (t, notifyInitial) =>
                       {
                           //1) notify when values have changed 
                           //2) resubscribe when changed because it may be a child object which has changed
                           var valueHasChanged = GetNotifiers(t, chain)
                                                .Merge()
                                                .Take(1)
                                                .Repeat();

                           if (notifyInitial)
                           {
                               valueHasChanged = Observable.Defer(() => Observable.Return(Unit.Default))
                                                           .Concat(valueHasChanged);
                           }

                           return valueHasChanged.Select(_ => GetPropertyValue(t, chain, valueAccessor));
                       };
        }

        public ObservablePropertyFactory(Expression<Func<TObject, TProperty>> expression)
        {
            //this overload is used for shallow observations i.e. depth = 1, so no need for re-subscriptions
            var member = expression.GetProperty();
            var accessor = expression.Compile();

            _factory = (t, notifyInitial) =>
                       {
                           PreviousPropertyValue<TObject, TProperty> Factory() => new PreviousPropertyValue<TObject, TProperty>(t, accessor(t));

                           var propertyChanged = Observable.FromEventPattern<PropertyChangingEventHandler, PropertyChangingEventArgs>(handler => t.PropertyChanging += handler,
                                                                                                                                      handler => t.PropertyChanging -= handler)
                                                           .Where(args => args.EventArgs.PropertyName == member.Name)
                                                           .Select(x => Factory());

                           if (!notifyInitial)
                           {
                               return propertyChanged;
                           }

                           var initial = Observable.Defer(() => Observable.Return(Factory()));

                           return initial.Concat(propertyChanged);
                       };
        }

        public IObservable<PreviousPropertyValue<TObject, TProperty>> Create(TObject source, bool notifyInitial) => _factory(source, notifyInitial);

        //create notifier for all parts of the property path 
        private static IEnumerable<IObservable<Unit>> GetNotifiers(TObject source, ObservablePropertyPart[] chain)
        {
            object value = source;

            foreach (var metadata in chain.Reverse())
            {
                var obs = metadata.Factory(value)
                                  .Publish()
                                  .RefCount();

                value = metadata.Accessor(value);

                yield return obs;

                if (value == null)
                {
                    yield break;
                }
            }
        }

        //walk the tree and break at a null, or return the value [should reduce this to a null an expression]
        private static PreviousPropertyValue<TObject, TProperty> GetPropertyValue(TObject source, ObservablePropertyPart[] chain, Func<TObject, TProperty> valueAccessor)
        {
            object value = source;

            foreach (var metadata in chain.Reverse())
            {
                value = metadata.Accessor(value);

                if (value == null)
                {
                    return new PreviousPropertyValue<TObject, TProperty>(source, default);
                }
            }

            return new PreviousPropertyValue<TObject, TProperty>(source, valueAccessor(source));
        }
    }

    internal sealed class ObservablePropertyPart
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly MemberExpression _expression;
        public Func<object, IObservable<Unit>> Factory { get; }
        public Func<object, object> Accessor { get; }

        public ObservablePropertyPart(MemberExpression expression)
        {
            _expression = expression;
            Factory = expression.CreatePropertyChangedFactory();
            Accessor = expression.CreateValueAccessor();
        }
    }
    
    /// <summary>
    /// Container holding sender and latest property value
    /// </summary>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public sealed class PreviousPropertyValue<TObject, TValue> : IEquatable<PreviousPropertyValue<TObject, TValue>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreviousPropertyValue{TObject,TValue}"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="value">The value.</param>
        public PreviousPropertyValue(TObject sender, TValue value)
        {
            Sender = sender;
            Value = value;
        }

        internal PreviousPropertyValue(TObject sender)
        {
            Sender = sender;
            UnobtainableValue = true;
            Value = default(TValue);
        }

        /// <summary>
        /// The Sender
        /// </summary>
        public TObject Sender { get; }

        /// <summary>
        /// Latest observed value
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// Flag to indicated that the value was unobtainable when observing a deeply nested struct
        /// </summary>
        internal bool UnobtainableValue { get; }

        /// <inheritdoc />
        public bool Equals(PreviousPropertyValue<TObject, TValue> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return EqualityComparer<TObject>.Default.Equals(Sender, other.Sender) && EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj is PreviousPropertyValue<TObject, TValue> && Equals((PreviousPropertyValue<TObject, TValue>)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<TObject>.Default.GetHashCode(Sender) * 397) ^ EqualityComparer<TValue>.Default.GetHashCode(Value);
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(PreviousPropertyValue<TObject, TValue> left, PreviousPropertyValue<TObject, TValue> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(PreviousPropertyValue<TObject, TValue> left, PreviousPropertyValue<TObject, TValue> right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Sender} ({Value})";
        }
    }