namespace Tree.Extensions

{
  static class ArrayExtensions
  {
    /// <summary>
    ///The method compare two arays and return true if element of the second array equals the elements of first array in the same order
    ///and the length of the first  array is >= then length of second array.
    ///So we can detect appropriate tree node and its children
    /// </summary>
    public static bool Compare(this int[] child, int[] parent)
    {
      if (child.Length >= parent.Length)
      {
        int n = 0;
        for (int i = 0; i < parent.Length; i++)
        {
          if (child[i] == parent[i]) n++;
        }
        if (n == parent.Length) return true;
      }
      return false;
    }

    /// <summary>
    ///The method compare two arays and return true if element of the second array equals the elements of first array in the same order
    ///and the length of the first array is > then length of second array.
    ///So we can detect children those are relevant to the tree node
    /// </summary>
    public static bool IsChildOf(this int[] child, int[] parent)
    {
      if (child.Length > parent.Length)
      {
        int n = 0;
        for (int i = 0; i < parent.Length; i++)
        {
          if (child[i] == parent[i]) n++;
        }
        if (n == parent.Length) return true;
      }
      return false;
    }

    /// <summary>
    ///The method compare two arays and return true if element of the second array equals the elements of first array in the same order
    ///and the length-1 of the first array is = to length of second array.
    ///So we can detect first cicle children those are relevant to the tree node
    /// </summary>
    public static bool IsFirstCicleChildOf(this int[] child, int[] parent)
    {
      if (child.Length - 1 == parent.Length)
      {
        int n = 0;
        for (int i = 0; i < parent.Length; i++)
        {
          if (child[i] == parent[i]) n++;
        }
        if (n == parent.Length) return true;
      }
      return false;
    }
  }
}
