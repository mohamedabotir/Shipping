using Common.Result;
using Common.ValueObject;

namespace Domain.ValueObject;

public class User: ValueObject<User>
{
    public string Name { get;private set; }
    public Address Address { get;private set; }
    public string PhoneNumber { get;private set; }

    private User(string name, Address address)
    {
         Name = name;
         Address = address;
    }

    public static Result<User> CreateInstance(Maybe<string> name, Address address)
    {
      return  name.ToResult("Name Cannot be Empty")
            .Map(e=>new User(e,address));
    }

    protected override bool EqualsCore(User other)
    {
       return other.Name == Name;
    }

    protected override int GetHashCodeCore()
    {
        
        return Name.GetHashCode() & Address.GetHashCode();
    }
}