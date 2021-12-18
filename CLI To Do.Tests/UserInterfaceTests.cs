using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;

namespace CLI_To_Do.Tests; 

public class UserInterfaceTests {
    [TestCase("2021-1-1", "2021-01-01")]
    [TestCase("2021-01-01", "2021-01-01")]
    [TestCase("2021-1-10", "2021-01-10")]
    [TestCase("2021-10-1", "2021-10-01")]
    [TestCase("2021-12-31", "2021-12-31")]
    [TestCase("invalid\n2021-12-31", "2021-12-31")]
    public void GetDate_Valid(string date, string result) {
        Console.SetIn(new StringReader(date));
        Assert.AreEqual(result, UserInterface.getDate());
    }

    [Test]
    public void GetDate_Today() {
        Console.SetIn(new StringReader("\n"));
        Assert.AreEqual(DateTime.Today.ToString("yyyy-MM-dd"), UserInterface.getDate());
    }
    
    [Test]
    public void GetDate_Tomorrow() {
        Console.SetIn(new StringReader("tomorrow"));
        Assert.AreEqual(DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"), UserInterface.getDate());
    }

    [Test]
    public void GetDate_Invalid() {
        Console.SetIn(new StringReader("Invalid"));
        Assert.Throws<NullReferenceException>(() => UserInterface.getDate());
    }
    
    [TestCase("3am", "3:00 AM")]
    [TestCase("3pm", "3:00 PM")]
    [TestCase("12am", "12:00 AM")]
    [TestCase("12pm", "12:00 PM")]
    public void GetTime_Valid(string input, string result) {
        Console.SetIn(new StringReader(input));
        Assert.AreEqual(result, UserInterface.getTime().ToString("t", CultureInfo.CreateSpecificCulture("en-us")));
    }

    [Test]
    public void GetTime_None() {
        Console.SetIn(new StringReader("\n"));
        Assert.AreEqual(new DateTime(), UserInterface.getTime());
    }

    [TestCase("18", "12:00 AM")] 
    [TestCase("24", "12:00 AM")] 
    [TestCase("0", "12:00 AM")]
    [TestCase("-1", "12:00 AM")] 
    [TestCase("25", "12:00 AM")] 
    [TestCase("26", "12:00 AM")] 
    public void GetTime_Invalid(string input, string result) {
        Console.SetIn(new StringReader(input));
        Assert.AreEqual(result, UserInterface.getTime().ToString("t", CultureInfo.CreateSpecificCulture("en-us")));
    }
    
    [TestCase("Title", "Title")]
    [TestCase("1234", "1234")]
    [TestCase("\ntitle2", "title2")]
    [TestCase("\n\n\n\ntitle4", "title4")]
    public void GetTitle_Valid(string title, string result) {
        Console.SetIn(new StringReader(title));
        Assert.AreEqual(result, UserInterface.getTitle());
    }
    
    [TestCase(10, "100\n5", 5)]
    [TestCase(10, "\n", 1)]
    [TestCase(10, "10", 10)]
    public void GetListsHelper_Valid(int total, string input, int result) {
        Console.SetIn(new StringReader(input));
        Assert.AreEqual(result, UserInterface.GetListsHelper(total));
    }
    
    [TestCase(10, "100", 1)]
    [TestCase(10, "\n", 1)]
    [TestCase(10, "test", 1)]
    public void GetListsHelper_Invalid(int total, string input, int result) {
        Console.SetIn(new StringReader(input));
        Assert.AreEqual(result, UserInterface.GetListsHelper(total));
    }
}