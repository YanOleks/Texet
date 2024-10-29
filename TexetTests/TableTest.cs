using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Texet.Models;
namespace TexetTests
{
    public class TableTest
    {
        private Table table;
        private string path ="";

        [SetUp]
        public void SetUp()
        {
            table = new Table();
        }
        [TearDown]
        public void TearDown()
        {
            File.Delete(path);
        }

        [Test]
        public void Test1()
        {
            table.Data.Clear();
            table.Data.Columns.Add("1");
            table.Data.Columns.Add("2");
            DataRow _ravi = table.Data.NewRow();
            _ravi["1"] = "1";
            _ravi["2"] = "2";
            table.Data.Rows.Add(_ravi);

            table.Save();

            string appDataPath = FileSystem.AppDataDirectory;
            string filename = System.IO.Path.Combine(appDataPath, "table.csv");

            path = filename;
            Assert.That(File.Exists(filename), Is.True);
            Assert.That(File.ReadAllText(filename), Is.EqualTo("\"1\",\"2\"\n"));

        }
    }
}
