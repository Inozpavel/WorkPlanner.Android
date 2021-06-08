namespace WorkPlanner.Requests
{
    public class UpdateProfileRequest
    {
        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Patronymic { get; set; }

        public string PhoneNumber { get; set; }
    }
}