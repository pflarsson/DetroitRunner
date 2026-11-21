namespace DetroitRunner.Tests;

public class MaximumSubarraySumTests
{
    // Kadane's algorithm: track the best sum ending at the current position
    // (extend the previous subarray, or start fresh at the current element,
    // whichever is larger) and the best sum seen overall. O(n) time / O(1) space.
    private static int MaxSubArray(int[] nums)
    {
        const int  FRIST_INDEX = 0;
        int bestEndingHere = nums[FRIST_INDEX];
        int best = nums[FRIST_INDEX];

        for (int i = 1; i < nums.Length; i++)
        {
            bestEndingHere = Math.Max(nums[i], bestEndingHere + nums[i]);
            best = Math.Max(best, bestEndingHere);
        }

        return best;
    }

    [Theory]
    [InlineData(new[] { -2, 1, -3, 4, -1, 2, 1, -5, 4 }, 6)]
    [InlineData(new[] { 1 }, 1)]
    [InlineData(new[] { 5, 4, -1, 7, 8 }, 23)]
    [InlineData(new[] { -1, -2, -3, -4 }, -1)]
    [InlineData(new[] { -3, -1, -2 }, -1)]
    public void MaxSubArray_ReturnsSumOfContiguousSubarrayWithLargestSum(int[] nums, int expected)
    {
        int actual = MaxSubArray(nums);

        Assert.Equal(expected, actual);
    }
}
