using System;
using System.Linq;
using NUnit.Framework;
using SoraTehk.Collections;

namespace SoraTehk.Tests {
    public class UniqueStackTests {
        [Test]
        public void AddAndRemove_ShouldMaintainStackIntegrity() {
            var stack = new UniqueStack<int>();
            stack.Add(1);
            stack.Add(2);
            stack.Push(3);
            stack.Remove(2);
            
            Assert.AreEqual(2, stack.Count);
            CollectionAssert.AreEqual(new[] { 3, 1 }, stack.ToArray());
        }
        
        [Test]
        public void Add_ShouldAddItem_WhenNotExists() {
            var stack = new UniqueStack<int>();
            stack.Add(1);
            
            Assert.AreEqual(1, stack.Count);
            Assert.IsTrue(stack.Contains(1));
        }
        
        [Test]
        public void Add_ShouldNotAddDuplicate() {
            var stack = new UniqueStack<int>();
            stack.Add(1);
            stack.Add(1);
            
            Assert.AreEqual(1, stack.Count);
        }
        
        [Test]
        public void Clear_ShouldRemoveAllItems() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            stack.Clear();
            
            Assert.AreEqual(0, stack.Count);
            Assert.IsFalse(stack.Contains(1));
        }
        
        [Test]
        public void Contains_ShouldReturnFalseForNonExistingItem() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            Assert.IsFalse(stack.Contains(5));
        }
        
        [Test]
        public void Contains_ShouldReturnTrueForExistingItem() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            Assert.IsTrue(stack.Contains(2));
        }
        
        [Test]
        public void Constructor_ShouldPopulateStackAndPreserveOrder() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3, 1, 4 });
            Assert.AreEqual(4, stack.Count);
            CollectionAssert.AreEqual(new[] { 4, 3, 2, 1 }, stack.ToArray());
        }
        
        [Test]
        public void Enumerator_ShouldIterateFromTopToBottom() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            var items = stack.ToArray();
            
            CollectionAssert.AreEqual(new[] { 3, 2, 1 }, items);
        }
        
        [Test]
        public void Enumerator_ShouldReflectChangesAfterInsertAndRemove() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            stack.Insert(1, 99);
            stack.RemoveAt(0);
            
            var items = stack.ToArray();
            CollectionAssert.AreEqual(new[] { 99, 2, 1 }, items);
        }
        
        [Test]
        public void IndexOf_ShouldReturnCorrectStackIndex() {
            var stack = new UniqueStack<int>(new[] { 10, 20, 30 });
            Assert.AreEqual(0, stack.IndexOf(30));
            Assert.AreEqual(2, stack.IndexOf(10));
            Assert.AreEqual(-1, stack.IndexOf(99));
        }
        
        [Test]
        public void IndexOf_ShouldReturnCorrectStackIndexAfterInsertions() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            stack.Insert(0, 4);
            stack.Insert(2, 5);
            
            Assert.AreEqual(0, stack.IndexOf(4));
            Assert.AreEqual(1, stack.IndexOf(3));
            Assert.AreEqual(2, stack.IndexOf(5));
            Assert.AreEqual(3, stack.IndexOf(2));
            Assert.AreEqual(4, stack.IndexOf(1));
        }
        
        [Test]
        public void Insert_ShouldHandleExistingItemAtSameIndex() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            stack.Insert(1, 2);
            
            Assert.AreEqual(3, stack.Count);
            CollectionAssert.AreEqual(new[] { 3, 2, 1 }, stack.ToArray());
        }
        
        [Test]
        public void Insert_ShouldInsertAtBottomWhenIndexCount() {
            var stack = new UniqueStack<int>(new[] { 1, 2 });
            stack.Insert(2, 99);
            
            Assert.AreEqual(3, stack.Count);
            Assert.AreEqual(2, stack.IndexOf(99));
            CollectionAssert.AreEqual(new[] { 2, 1, 99 }, stack.ToArray());
        }
        
        [Test]
        public void Insert_ShouldInsertAtCorrectStackIndex() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            stack.Insert(1, 99);
            
            Assert.AreEqual(4, stack.Count);
            Assert.AreEqual(0, stack.IndexOf(3));
            Assert.AreEqual(1, stack.IndexOf(99));
            Assert.AreEqual(2, stack.IndexOf(2));
        }
        
        [Test]
        public void Insert_ShouldInsertAtTopWhenIndexZero() {
            var stack = new UniqueStack<int>(new[] { 1, 2 });
            stack.Insert(0, 99);
            
            Assert.AreEqual(3, stack.Count);
            Assert.AreEqual(0, stack.IndexOf(99));
            CollectionAssert.AreEqual(new[] { 99, 2, 1 }, stack.ToArray());
        }
        
        [Test]
        public void Insert_ShouldMoveExistingItemToNewIndex() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            stack.Insert(0, 2);
            
            Assert.AreEqual(3, stack.Count);
            Assert.AreEqual(0, stack.IndexOf(2));
            Assert.AreEqual(1, stack.IndexOf(3));
            Assert.AreEqual(2, stack.IndexOf(1));
        }
        
        [Test]
        public void Pop_ShouldReturnTopItemAndRemoveIt() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            int top = stack.Pop();
            
            Assert.AreEqual(3, top);
            Assert.AreEqual(2, stack.Count);
            Assert.IsFalse(stack.Contains(3));
        }
        
        [Test]
        public void Pop_ShouldWorkCorrectlyAfterInsertions() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            stack.Insert(1, 99);
            int top = stack.Pop();
            
            Assert.AreEqual(3, top);
            Assert.AreEqual(3, stack.Count);
            CollectionAssert.AreEqual(new[] { 99, 2, 1 }, stack.ToArray());
        }
        
        [Test]
        public void Pop_ShouldThrowException_WhenEmpty() {
            var stack = new UniqueStack<int>();
            Assert.Throws<InvalidOperationException>(() => stack.Pop());
        }
        
        [Test]
        public void Push_ShouldAddItemOnTop() {
            var stack = new UniqueStack<int>(new[] { 1, 2 });
            stack.Push(3);
            
            Assert.AreEqual(3, stack.Count);
            Assert.AreEqual(0, stack.IndexOf(3));
        }
        
        [Test]
        public void Push_ShouldHandleMultipleDuplicatesCorrectly() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            stack.Push(2);
            stack.Push(2);
            
            Assert.AreEqual(3, stack.Count);
            Assert.AreEqual(0, stack.IndexOf(2));
            CollectionAssert.AreEqual(new[] { 2, 3, 1 }, stack.ToArray());
        }
        
        [Test]
        public void Push_ShouldMoveExistingItemToTop() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            stack.Push(2);
            
            Assert.AreEqual(3, stack.Count);
            Assert.AreEqual(0, stack.IndexOf(2));
            Assert.AreEqual(1, stack.IndexOf(3));
            Assert.AreEqual(2, stack.IndexOf(1));
        }
        
        [Test]
        public void Remove_ShouldRemoveExistingItem() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            bool removed = stack.Remove(2);
            
            Assert.IsTrue(removed);
            Assert.AreEqual(2, stack.Count);
            Assert.IsFalse(stack.Contains(2));
        }
        
        [Test]
        public void Remove_ShouldReturnFalseForNonExistingItem() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            bool removed = stack.Remove(99);
            
            Assert.IsFalse(removed);
            Assert.AreEqual(3, stack.Count);
        }
        
        [Test]
        public void RemoveAt_ShouldRemoveItemAtStackIndex() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            stack.RemoveAt(1);
            
            Assert.AreEqual(2, stack.Count);
            CollectionAssert.AreEqual(new[] { 3, 1 }, stack.ToArray());
        }
        
        [Test]
        public void RemoveAt_ShouldThrowException_WhenInvalidIndex() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            Assert.Throws<ArgumentOutOfRangeException>(() => stack.RemoveAt(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => stack.RemoveAt(3));
        }
        
        [Test]
        public void TryPeek_ShouldReturnFalse_WhenEmpty() {
            var stack = new UniqueStack<int>();
            Assert.IsFalse(stack.TryPeek(out int _));
        }
        
        [Test]
        public void TryPeek_ShouldReturnTopItemWithoutRemoving() {
            var stack = new UniqueStack<int>(new[] { 1, 2, 3 });
            Assert.IsTrue(stack.TryPeek(out int top));
            Assert.AreEqual(3, top);
            Assert.AreEqual(3, stack.Count);
        }
    }
}