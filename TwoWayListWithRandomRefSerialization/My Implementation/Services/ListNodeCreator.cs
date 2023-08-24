using System.Globalization;
using Task_Original_Files;
namespace Logic_Layer.Services;

public static class ListNodeCreator
{
    public const int MAX_RECOMMENDED_LIST_SIZE = 100000;
    public const int OPTIMAL_LARGE_LIST_SIZE = 10000;
    public const int PREDEFINED_LIST_SIZE = 13;

    public static ListNode CreateList(int requiredListSize)
    {
        if (requiredListSize == PREDEFINED_LIST_SIZE)
            return CreateSmallGeneralList();

        if (requiredListSize > MAX_RECOMMENDED_LIST_SIZE)
            throw new ArgumentException(
                $"Required list size exceeds MAX RECOMMENDED LIST SIZE which is: {MAX_RECOMMENDED_LIST_SIZE}");
        
        var dictionary = new Dictionary<int, ListNode>(requiredListSize);
        var head =  new ListNode();
        head.Data = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
        dictionary.Add(0, head);
        
        for (int i = 1; i < requiredListSize; i++)
        {
            var node = new ListNode();
            node.Data = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            node.Previous = dictionary[i - 1];
            dictionary.Add(i, node);
            dictionary[i - 1].Next = dictionary[i];
        }
        
        var randomizer = new Random(DateTime.UtcNow.Millisecond);

        for (int i = 0; i < requiredListSize; i++)
        {
            int randomId = randomizer.Next(0, requiredListSize - 1);
            dictionary[i].Random = dictionary[randomId];
        }

        return dictionary[0];
    }
    
    public static ListNode CreateSmallGeneralList()
    {
        var node13 = new ListNode();
        node13.Data = "The Elder Scrolls";

        var node12 = new ListNode();
        node12.Data = "Serious Sam";
        node12.Next = node13;
        node12.Random = node12;
        node13.Previous = node12;

        var node11 = new ListNode();
        node11.Data = "Call of Cthulhu"; 
        node11.Next = node12;
        node12.Previous = node11;

        var node10 = new ListNode();
        node10.Data = "Mass Effect";
        node10.Next = node11;
        node11.Previous = node10;

        var node9 = new ListNode();
        node9.Data = "World of Warcraft";
        node9.Next = node10;
        node10.Previous = node9;
        node9.Random = node13;
        node13.Random = node9;

        var node8 = new ListNode();
        node8.Data = "Aliens vs Predator";
        node8.Next = node9;
        node9.Previous = node8;
        node8.Random = node10;

        var node7 = new ListNode();
        node7.Data = "Pathologic";
        node7.Next = node8;
        node8.Previous = node7;

        var node6 = new ListNode();
        node6.Data = "Jericho";
        node6.Next = node7;
        node7.Previous = node6;
        node6.Random = node12;
        node11.Random = node6;

        var node5 = new ListNode();
        node5.Data = "Black Moon Chronicles";
        node5.Next = node6;
        node6.Previous = node5;

        var node4 = new ListNode();
        node4.Data = "StarCraft";
        node4.Next = node5;
        node5.Previous = node4;
        node5.Random = node4;

        var node3 = new ListNode();
        node3.Data = "Fallout";
        node3.Next = node4;
        node4.Previous = node3;

        var node2 = new ListNode();
        node2.Data = "Phoenix Point";
        node2.Next = node3;
        node3.Previous = node2;

        var node1 = new ListNode();
        node1.Data = "XCOM";
        node1.Next = node2;
        node2.Previous = node1;
        node1.Random = node4;
        node4.Random = node1;
        node2.Random = node1;

        return node1;
    }

    public static ListNode CreateSmallProblematicList()
    {
        var node13 = new ListNode();
        node13.Data = "The Elder Scrolls";

        var node12 = new ListNode();
        node12.Data = "Serious Sam";
        node12.Next = node13;
        node12.Random = node13;
        node13.Previous = node12;

        var node11 = new ListNode();
        node11.Data = "Call of Cthulhu"; 
        node11.Next = node12;
        node12.Previous = node11;

        var node10 = new ListNode();
        node10.Data = "Mass Effect";
        node10.Next = node11;
        node11.Previous = node10;
        node10.Random = node13;

        var node9 = new ListNode();
        node9.Data = "World of Warcraft";
        node9.Next = node10;
        node10.Previous = node9;
        node9.Random = node13;
        node13.Random = node9;

        var node8 = new ListNode();
        node8.Data = "Aliens vs Predator";
        node8.Next = node9;
        node9.Previous = node8;
        node8.Random = node13;

        var node7 = new ListNode();
        node7.Data = "Pathologic";
        node7.Next = node8;
        node7.Random = node13;
        node8.Previous = node7;

        var node6 = new ListNode();
        node6.Data = "Jericho";
        node6.Next = node7;
        node7.Previous = node6;
        node6.Random = node13;
        node11.Random = node13;

        var node5 = new ListNode();
        node5.Data = "Black Moon Chronicles";
        node5.Next = node6;
        node6.Previous = node5;

        var node4 = new ListNode();
        node4.Data = "StarCraft";
        node4.Next = node5;
        node5.Previous = node4;
        node5.Random = node13;

        var node3 = new ListNode();
        node3.Data = "Fallout";
        node3.Next = node4;
        node4.Previous = node3;
        node3.Random = node13;

        var node2 = new ListNode();
        node2.Data = "Phoenix Point";
        node2.Next = node3;
        node3.Previous = node2;

        var node1 = new ListNode();
        node1.Data = "XCOM";
        node1.Next = node2;
        node2.Previous = node1;
        node1.Random = node4;
        node4.Random = node13;
        node2.Random = node1;

        return node1;
    }
}