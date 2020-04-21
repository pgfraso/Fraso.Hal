namespace Fraso.Hal.Conversions
{
    public sealed class PolicyRuleCoupling<TWrappedObject, TRule>
    {
        internal readonly WrapPolicy<TWrappedObject> Policy;

        internal readonly TRule Rule;

        internal PolicyRuleCoupling(
            WrapPolicy<TWrappedObject> 
            policy, TRule rule)
        {
            Policy = policy;
            Rule = rule;
        }
    }

    internal static class PolicyRuleCoupling
    {
        public static PolicyRuleCoupling<TPolicy, TRule> From<TPolicy, TRule>(
            WrapPolicy<TPolicy> policy, 
            TRule rule)
            => new PolicyRuleCoupling<TPolicy, TRule>(policy, rule);
    }
}
