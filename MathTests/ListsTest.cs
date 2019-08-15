using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using ECS;
using ECS.Experimental;
using Math.Vectors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MathTests
{
    [TestClass]
    public class ListsTest
    {
        private struct TestStruct
        {
            public TestStruct(bool delete)
            {
                Delete = delete;
                V1 = Vector3.Zero;
                V2 = Vector3.Zero;
                V3 = Vector3.Zero;
            }

            public bool Delete;
            private Vector3 V1;
            private Vector3 V2;
            private Vector3 V3;
            
        }

        [TestMethod]
        public void TestListPerformance()
        {
            var sw = new Stopwatch();

            List<TestStruct> structs;

            sw.Restart();
            {
                structs = new List<TestStruct>(32);

                for (int i = 0; i < 100000; ++i)
                {
                    structs.Add(new TestStruct(i%3 == 0));
                }

                
            }
            sw.Stop();
            
            Console.WriteLine("List add: " + sw.ElapsedMilliseconds);

            sw.Restart();
            {
                for (int i = 0, ilen = structs.Count; i < ilen; ++i)
                {
                    var x = structs[i];
                }
            }
            sw.Stop();

            Console.WriteLine("List 1[]: " + sw.ElapsedMilliseconds);

            sw.Restart();
            {
                for (int i = 0; i < structs.Count; ++i)
                {
                    if (!structs[i].Delete)
                    {
                        continue;
                    }

                    structs.RemoveAt(i);
                    i--;

                }
            }
            sw.Stop();

            Console.WriteLine("List remove: " + sw.ElapsedMilliseconds);

            sw.Restart();
            {
                for (int i = 0, ilen = structs.Count; i < ilen; ++i)
                {
                    var x = structs[i];
                }
            }
            sw.Stop();

            Console.WriteLine("List 2[]: " + sw.ElapsedMilliseconds);
        }

        [TestMethod]
        public void TestComponentsListPerformance()
        {
            var sw = new Stopwatch();

            ComponentsArray<TestStruct> structs;

            sw.Restart();
            {
                structs = new ComponentsArray<TestStruct>(32, 100000);

                for (int i = 0; i < 100000; ++i)
                {
                    var x = new TestStruct(i % 3 == 0);
                    structs.Add(i, ref x);
                }


            }
            sw.Stop();

            Console.WriteLine("ComponentsList add: " + sw.ElapsedMilliseconds);

            sw.Restart();
            {
                for (int i = 0, ilen = structs.Length; i < ilen; ++i)
                {
                    var x = structs.GetValue(i);
                }
            }
            sw.Stop();

            Console.WriteLine("ComponentsList 1[]: " + sw.ElapsedMilliseconds);

            sw.Restart();
            {
                var x = new TestStruct();
                for (int i = 0, ilen = structs.Length; i < ilen; ++i)
                {
                    if (structs.TryGetValue(i, ref x))
                    {
                        if (x.Delete)
                        {
                            structs.Remove(i);
                        }
                    }
                }
            }
            sw.Stop();

            Console.WriteLine("ComponentsList remove: " + sw.ElapsedMilliseconds);

            sw.Restart();
            {
                for (int i = 0, ilen = structs.Length; i < ilen; ++i)
                {
                    var x = structs.GetValue(i);
                }
            }
            sw.Stop();

            Console.WriteLine("ComponentsList 2[]: " + sw.ElapsedMilliseconds);
        }
    }
}
