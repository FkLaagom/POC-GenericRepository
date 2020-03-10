using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace generic_repository
{
    public class Person
    {
        public string Name { get; set; }
    }

    public interface IRepository<T> where T : class
    {
        IEnumerable<T> Get();
        IEnumerable<T> Get(Func<T, bool> predicate);
        void Delete(T obj);
        void Add(T obj);
        void Add(IEnumerable<T> obj);
    }

    public class InMemmoryRepository<T> : IRepository<T> where T : class
    {
        private List<T> _items = new List<T>();
        public IEnumerable<T> Get() => _items;
        public IEnumerable<T> Get(Func<T, bool> predicate)
            => _items.Where(predicate).AsEnumerable<T>();
        public void Add(T obj) => _items.Add(obj);
        public void Add(IEnumerable<T> obj) => _items.AddRange(obj);
        public void Delete(T obj) => _items.Remove(obj);
    }

    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IRepository<Person>, InMemmoryRepository<Person>>()
                .BuildServiceProvider();

            var db = serviceProvider.GetService<IRepository<Person>>();

            db.Add(new[]
            {
                new Person { Name = "Olof" },
                new Person { Name = "Benjamin" },
                new Person { Name = "Chi" }
            });

            var persons = db.Get();
            var olof = db.Get(x => x.Name == "Olof").FirstOrDefault();
            var notOlof = db.Get(x => x.Name != "Olof").ToList();

            db.Delete(olof);
            var notOlof2 = db.Get();
        }
    }
}
