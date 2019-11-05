using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using WeShare.PropertyWrapper;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                var user = new User();
                for (int i = 0; i < 100000; i++)
                {
                    var type = user.GetType();
                    //foreach (var propertyInfo in type.GetProperties())
                    //{

                    //    propertyInfo.SetValue(user, Convert.ChangeType("123", propertyInfo.PropertyType));
                    //    propertyInfo.GetValue(user);
                    //}

                    var pros = CacheService.Get<List<PropertyEntity>>(type.FullName);
                    if (pros == null)
                    {
                        pros = new List<PropertyEntity>();
                        foreach (var propertyInfo in type.GetProperties())
                        {
                            pros.Add(new PropertyEntity
                            {
                                PropertyInfo = propertyInfo,
                                HasAttr = propertyInfo.IsDefined(typeof(MyAttr)),
                                PropertyType = propertyInfo.PropertyType,
                                SetMethod = propertyInfo.CreateSetter(),
                                GetMethod = propertyInfo.CreateGetter()
                            });
                        }
                        CacheService.Add(type.FullName, pros);
                    }

                    foreach (var pro in pros)
                    {
                        pro.Set(user, "123");
                        pro.Get(user);
                        //pro.PropertyInfo.SetValue(user, Convert.ChangeType("123", pro.PropertyType));
                        //pro.PropertyInfo.GetValue(user);
                    }

                }
                watch.Stop();
                Console.WriteLine(watch.Elapsed.TotalMilliseconds);
                Console.ReadKey();
            }

        }
    }
    public class PropertyEntity
    {
        public PropertyInfo PropertyInfo { get; set; }
        public Type PropertyType { get; set; }
        public bool HasAttr { get; set; }
        public ISetValue SetMethod { get; set; }
        public IGetValue GetMethod { get; set; }

        public void Set(object obj, object val)
        {
            this.SetMethod.Set(obj, val);
        }

        public object Get(object obj)
        {
           return this.GetMethod.Get(obj);
        }
    }

    public class MyAttr : Attribute
    {

    }
    public class User
    {
        public int Id { get; set; }
        public int? Age { get; set; }
        public int Sex { get; set; }
        [MyAttr]
        public string Name { get; set; }
        public string Home { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
    }
}
