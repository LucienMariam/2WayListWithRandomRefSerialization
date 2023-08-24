using Logic_Layer.Services.Abstractions;
using Task_Original_Files;

namespace Logic_Layer.Strategies.Abstractions;

public abstract class DeepCopyListNodeStrategy : IDeepCopyStrategy<ListNode>
{
    public abstract ListNode DeepCopy(ListNode copyFrom);

    protected virtual ListNode DeepCopyFirstElement(ListNode copyFrom)
    {
        var copyTo = new ListNode();
        copyTo.Data = copyFrom.Data;
        copyTo.Random = copyFrom.Random == null ? null : copyTo;
        return copyTo;
    }
}