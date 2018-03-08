using System;
using System.Collections.Generic;
using DAPIWrapper.DAPIWrapper;
using System.IO;
using System.Web.Configuration;
using System.Web.Http;
using System.Web;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Drawing;
using System.Text;
using System.Windows.Markup;
using System.Linq.Expressions;

namespace ConsoleApp1
{
    class Program
    {




        static void Main(string[] args)
        {

            void Foo(StringBuilder x)
            {
                x = null;
            }
            Func<int> myFunc = () => 10;
            StringBuilder y = new StringBuilder();
            y.Append("hello");
            Expression<Func<int>> myExpression = () => 10;
            Foo(y);
            Console.WriteLine(myExpression);
            Console.ReadKey();




        }
    }
}
