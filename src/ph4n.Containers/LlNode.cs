using JetBrains.Annotations;

namespace ph4n.Containers
{
    sealed class LlNode<T>
    {
        [CanBeNull]
        public T Value { get; private set; }

        [CanBeNull] public LlNode<T> Prev;

        [CanBeNull] public LlNode<T> Next;

        public LlNode([CanBeNull] T value, [CanBeNull] LlNode<T> prev = null, [CanBeNull] LlNode<T> next = null)
        {
            Value = value;
            Prev = prev;
            Next = next;
        }

        public static LlNode<T> operator ++(LlNode<T> node)
        {
            return node == null ? null : node.Next;
        }

        public static LlNode<T> operator --(LlNode<T> node)
        {
            return node == null ? null : node.Prev;
        }
    }
}
