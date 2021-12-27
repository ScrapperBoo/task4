using System;
using System.Collections.Generic;
using System.Linq;

namespace DI
{
    public class DI
    {
        private List<SD> dependencies;
        public DI()
        {
            dependencies = new List<SD>();
        }
        public void AddUnstable<TService, TImplementation>()
        {
            dependencies.Add(new SD(typeof(TService), typeof(TImplementation), ST.Transient));
        }
        public void AddSingleton<TService, TImplementation>()
        {
            dependencies.Add(new SD(typeof(TService), typeof(TImplementation), ST.Singleton));
        }

        public object Get(Type serviceType)
        {
            List<Type> listik = new List<Type>();

            return Get(serviceType, listik);
        }

        //public T Get<T>() => (T)Get(typeof(T));
        //public object Get(Type sT)
        public object Get(Type serviceType, List<Type> parlist)
        {
            
            var specifier = dependencies.SingleOrDefault(x => x.ServiceType == serviceType);
            if (specifier == null)
            {
                throw new Exception("not found");
            }
            if (specifier.Implementation != null)
            {
                return specifier.Implementation;
            }



            var relevantType = specifier.ImplementationType;
            var constInfo = relevantType.GetConstructors().First();
            List<object> dar = new List<object>();

            foreach (var parameter in constInfo.GetParameters())
            {
                if (parlist.Contains(serviceType))
                {
                    throw new Exception("Цикл");
                }
                parlist.Add(serviceType);

                var newParameter = Get(parameter.ParameterType, parlist);

                parlist.Remove(serviceType);

                dar.Add(newParameter);
            }
            var parameters = dar.ToArray();
            var implementation = Activator.CreateInstance(relevantType, parameters);
            if (specifier.LifeTime == ST.Singleton)
            {
                specifier.Implementation = implementation;
            }
            return implementation;
        

        }
    }
}
