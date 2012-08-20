using System.Web.Mvc;
using Bevaq.RazorJs.Sample.Models;

namespace Bevaq.RazorJs.Sample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Vehicle[] vehicles = new[]
            {
                new Vehicle
                {
                    Name = "Schinn Fixed Bicycle",
                    Type = "Bicycle",
                    Price = 199.99m,
                    Components = new[]
                    {
                        new Component {Name = "Speed Gauge", Color = "Red"},
                        new Component {Name = "Pedal", Color = "Black"},
                        new Component {Name = "Big Wheel", Color = "Dark Red"}
                    }
                },
                new Vehicle
                {
                    Name = "Predator",
                    Type = "Fighter Drone",
                    Price = 1650000.99m,
                    Components = new[]
                    {
                        new Component {Name = "Autopilot", Color = "None"},
                        new Component {Name = "Missile Battery", Color = "Black"},
                    }
                },
            };
            return View(vehicles);
        }

        public ActionResult MoreVehicles()
        {
            var vehicles = new[]
            {
                new Vehicle
                {
                    Name = "1968 Pontiac Firebird",
                    Type = "Classic Car",
                    Price = 27499.99m,
                    Components = new[]
                    {
                        new Component {Name = "8 cyl. Engine", Color = "Red"},
                        new Component {Name = "Interiors", Color = "Cameo Ivory"},
                        new Component {Name = "Exteriors", Color = "Autumn Bronze"}
                    }
                },
                new Vehicle
                {
                    Name = "WW-II Seehund U-Boat",
                    Type = "German Midget Submarine",
                    Price = 2579000m,
                    Components = new[]
                    {
                        new Component {Name = "Torpedo", Color = "Gray"},
                        new Component {Name = "Torpedo", Color = "Gray"},
                        new Component {Name = "Torpedo", Color = "Gray"},
                        new Component {Name = "Torpedo", Color = "Black"},
                    }
                },
            };
            return Json(vehicles, JsonRequestBehavior.AllowGet);
        }
    }
}