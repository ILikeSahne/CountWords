using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CountWords;

namespace CountWordsTests
{
    [TestClass]
    public class CountWordsTest
    {
        /// <summary>
        /// Small test for the provided sample
        /// </summary>
        [TestMethod]
        public void Test_ProcessWords()
        {
            CountWords.CountWords countWords = new CountWords.CountWords("");
            countWords.ProcessWords("1:1 Adam Seth Enos\n1:2 Cainan Adam Seth Iared");

            Assert.AreEqual(countWords.WordCount["Adam"], 2);
            Assert.AreEqual(countWords.WordCount["Seth"], 2);
            Assert.AreEqual(countWords.WordCount["Enos"], 1);
            Assert.AreEqual(countWords.WordCount["1:2"], 1);
        }
    }
}
