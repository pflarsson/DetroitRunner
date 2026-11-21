namespace DetroitRunner.Tests;

public class TwoSumTests
{
    // Classic "Two Sum" using a hash map: for each number, check whether the
    // complement (target - number) was already seen, storing each value's
    // index as we go. This is O(n) time / O(n) space, versus O(n^2) for the
    // brute-force nested-loop approach.
    private static int[] TwoSum(int[] nums, int target)
    {
        var seen = new Dictionary<int, int>();

        for (int i = 0; i < nums.Length; i++)
        {
            var complement = target - nums[i];
            if (seen.TryGetValue(complement, out int complementIndex))
            {
                return [complementIndex, i];
            }

            seen[nums[i]] = i;
        }

        throw new InvalidOperationException("No two sum solution exists.");
    }

    [Theory]
    [InlineData(new[] { 2, 7, 11, 15 }, 9, new[] { 0, 1 })]
    [InlineData(new[] { 3, 2, 4 }, 6, new[] { 1, 2 })]
    [InlineData(new[] { 3, 3 }, 6, new[] { 0, 1 })]
    [InlineData(new[] { -1, -2, -3, -4, -5 }, -8, new[] { 2, 4 })]
    public void TwoSum_ReturnsIndicesOfTheTwoNumbersThatAddToTarget(int[] nums, int target, int[] expected)
    {
        var actual = TwoSum(nums, target);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TwoSum_ThrowsWhenNoPairSumsToTarget()
    {
        int[] nums = [1, 2, 3];

        Assert.Throws<InvalidOperationException>(() => TwoSum(nums, 100));
    }
}
