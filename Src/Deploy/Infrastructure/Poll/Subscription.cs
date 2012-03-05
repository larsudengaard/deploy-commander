namespace Deploy.Infrastructure.Poll
{
    public class Subscription
    {
        public Subscription(string channel, string @group)
        {
            Channel = channel;
            Group = group;
        }

        public string Channel { get; set; }
        public string Group { get; set; }

        public bool Equals(Subscription other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Equals(other.Channel, Channel) && Equals(other.Group, Group);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof (Subscription))
                return false;
            return Equals((Subscription) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Channel != null ? Channel.GetHashCode() : 0)*397) ^ (Group != null ? Group.GetHashCode() : 0);
            }
        }
    }
}