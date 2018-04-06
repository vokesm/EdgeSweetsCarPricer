using System;
using NUnit.Framework;

/*************************************************************************************
 * Author: Michael Vokes
 * Date: 4/5/2018
 * This algorithm was written as part of a code test taken from Edge Sweets website.
 * www.edge-sweets.com/software/codetest 
 ************************************************************************************/

namespace EdgeSweetsCarPricer
{
    //Unused, this buildable project was only used to test the assertions at the end of the file
    class Program
    {
        static void Main(string[] args){}
    }

    public class Car
    {
        public decimal PurchaseValue { get; set; }
        public int AgeInMonths { get; set; }
        public int NumberOfMiles { get; set; }
        public int NumberOfPreviousOwners { get; set; }
        public int NumberOfCollisions { get; set; }
    }

    public class PriceDeterminator
    {

        public decimal DetermineCarPrice(Car car)
        {
            bool bonusReceived = car.NumberOfPreviousOwners == 0;   //Determine if the bonus will be added at the end of calculation
            bool penaltyReceived = car.NumberOfPreviousOwners > 2;  //Determine if the value will be dropped because of multiple owners

            decimal multipleOwnersPenalty = 0.75m;                  
            decimal noPrevOwnersBonus = 1.1m;

            var newCarValue = DeterminePriceByAge(car.PurchaseValue, car.AgeInMonths);  //Calculate the price based on the age of the car
            newCarValue = DeterminePriceByMiles(newCarValue, car.NumberOfMiles);        //Calculate the price based on the mileage of the car

            if (bonusReceived)  //No previous owners
            {
                newCarValue = DeterminePriceByCollision(newCarValue, car.NumberOfCollisions);   //Determine price of the car based on the amount of collisions
                newCarValue *= noPrevOwnersBonus;                                               //Apply Bonus
            } else if (penaltyReceived)     //More than 2 previous owners
            {
                newCarValue *= multipleOwnersPenalty;                                           //Apply penalty
                newCarValue = DeterminePriceByCollision(newCarValue, car.NumberOfCollisions);   //Determine price of the car based on the amount of collisions
            }
            else
            {
                newCarValue = DeterminePriceByCollision(newCarValue, car.NumberOfCollisions);   //Determine price of the car based on the amount of collisions
            }
                
            return Math.Round(newCarValue, 2);  //Return rounded final price of the car
        }

        /*
         * This method determines the price of the car given the purchase value and the age in months of the car.
         */
        public decimal DeterminePriceByAge(decimal purchaseValue, int age)
        {
            int maxCarAgeInMonths = 10 * 12;
            decimal percentageOfValueLost = 0.005m;
            var carValue = purchaseValue;

            if (age > maxCarAgeInMonths) //If car is older than 10 years, calculate the lowest possible value based on age
            {
                carValue *= (1 - (percentageOfValueLost * maxCarAgeInMonths));
            }
            else //Car is less than 10 years old
            {
                carValue *= (1 - (percentageOfValueLost * age));
            }

            return carValue;
        }

        /*
         * This method determines the price of the car based on the given value and the mileage.
         */
        public decimal DeterminePriceByMiles(decimal currentValue, int numberOfMiles)
        {
            int maxMiles = 150000;
            decimal percentageOfValueLost = 0.002m;
            var carValue = currentValue;

            if (numberOfMiles > maxMiles)   //Calculate the lowest possible value based on miles
            {
                carValue *= (1 - (percentageOfValueLost * (maxMiles / 1000)));
            }
            else //Calculate new value based on miles
            {
                carValue *= (1 - (percentageOfValueLost * (numberOfMiles / 1000)));
            }

            return carValue;
        }

        /*
         * This method determines the price of the car based on the given value and the number of collisions
         */ 
        public decimal DeterminePriceByCollision(decimal currentValue, int numberOfCollisions)
        {
            int maxCollisions = 5;
            decimal percentageOfValueLost = 0.02m;
            var carValue = currentValue;

            if (numberOfCollisions > maxCollisions) //If car has been in more than 5 accidents, calculate the lowest possible value
            {
                carValue *= (1 - (percentageOfValueLost * maxCollisions));  //Every collision the current value of the car drops by 2%
            }
            else //Calculate the new value based on the number of collisions
            {
                carValue *= (1 - (percentageOfValueLost * numberOfCollisions));  //Every collision the current value of the car drops by 2%
            }

            return carValue;
        }
    }

    [TestFixture]
    public class UnitTests
    {
        [Test]
        public void CalculateCarValue()
        {
            AssertCarValue(25313.40m, 35000m, 3 * 12, 50000, 1, 1);
            AssertCarValue(19688.20m, 35000m, 3 * 12, 150000, 1, 1);
            AssertCarValue(19688.20m, 35000m, 3 * 12, 250000, 1, 1);
            AssertCarValue(20090.00m, 35000m, 3 * 12, 250000, 1, 0);
            AssertCarValue(21657.02m, 35000m, 3 * 12, 250000, 0, 1);
        }

        private static void AssertCarValue(decimal expectValue, decimal purchaseValue,
        int ageInMonths, int numberOfMiles, int numberOfPreviousOwners, int
        numberOfCollisions)
        {
            Car car = new Car
            {
                AgeInMonths = ageInMonths,
                NumberOfCollisions = numberOfCollisions,
                NumberOfMiles = numberOfMiles,
                NumberOfPreviousOwners = numberOfPreviousOwners,
                PurchaseValue = purchaseValue
            };
            PriceDeterminator priceDeterminator = new PriceDeterminator();
            var carPrice = priceDeterminator.DetermineCarPrice(car);
            Assert.AreEqual(expectValue, carPrice);
        }
    }
}
