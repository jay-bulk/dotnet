namespace HandsOnEx.Models
{
    public static class FavDestinationList
    {
        private static List<FavDestination> favDestinations = new List<FavDestination>();

        public static IEnumerable<FavDestination> FavDestinations()
        {
            return favDestinations;
        }

        public static void AddFavDestination(FavDestination destination)
        {
            favDestinations.Add(destination);
        }
    }

}
