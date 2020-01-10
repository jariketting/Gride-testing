using Gride.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrideTest
{
    public class GeneratorFunctions
    {
		public static List<EmployeeModel> resultAfterAvailabilityFilter = new List<EmployeeModel>();
		public static List<EmployeeModel> resultAfterFunctionFilter = new List<EmployeeModel>();
		public static List<EmployeeModel> resultAfterLocationFilter = new List<EmployeeModel>();
		public static List<EmployeeModel> resultAfterSkillFilter = new List<EmployeeModel>();

		public static List<EmployeeModel> employees = new List<EmployeeModel>()
		{
			new EmployeeModel{
					ID = 1,
					Name ="Guus",
					LastName ="Joppe",
					DoB = new DateTime(1998, 05,18),
					Gender =0,
					EMail ="0967844@hr.nl",
					PhoneNumber ="0640643724",
					Admin =true,
					Experience = 5,
					ProfileImage ="profile_0967844.jpeg"
				},
				new EmployeeModel
				{
					ID = 2,
					Name = "John",
					LastName = "Doe",
					DoB = new DateTime(1998, 05,18),
					Gender = 0,
					EMail = "0123456@hr.nl",
					PhoneNumber = "0612345678",
					Admin = false,
					Experience = 2,
					ProfileImage = "profile_0123456.jpeg",
					SupervisorID = 1
				},
				new EmployeeModel{
					ID = 3,
					Name ="Charles",
					LastName ="Babbage",
					DoB = new DateTime(1998, 05,18),
					Gender =0,
					EMail ="cb@hr.nl",
					PhoneNumber ="0640643724",
					Admin =false,
					Experience =3,
					ProfileImage ="",
					SupervisorID = 1
				},
				new EmployeeModel{
					ID = 4,
					Name ="Bill",
					LastName ="Gates",
					DoB = new DateTime(1998, 05,18),
					Gender =0,
					EMail ="bg@hr.nl",
					PhoneNumber ="0640643724",
					Admin =false,
					Experience =4,
					ProfileImage ="",
					SupervisorID = 1
				},
				new EmployeeModel{
					ID = 5,
					Name ="Steve",
					LastName ="Jobs",
					DoB = new DateTime(1998, 05,18),
					Gender =0,
					EMail ="sj@hr.nl",
					PhoneNumber ="0640643724",
					Admin = false,
					Experience =1,
					ProfileImage ="",
					SupervisorID = 1
				},
				new EmployeeModel{
					ID = 6,
					Name ="Ava",
					LastName ="Lovelace",
					DoB = new DateTime(1998, 05,18),
					Gender = Gender.Female,
					EMail ="al@hr.nl",
					PhoneNumber ="0640643724",
					Admin = false,
					Experience =1,
					ProfileImage ="",
					SupervisorID = 1
				},
				new EmployeeModel{
					ID = 7,
					Name ="Elon",
					LastName ="Musk",
					DoB = new DateTime(1998, 05,18),
					Gender =0,
					EMail ="em@hr.nl",
					PhoneNumber ="0640643724",
					Admin = false,
					Experience =1,
					ProfileImage ="",
					SupervisorID = 1
				},
				new EmployeeModel{
					ID = 8,
					Name ="Guido",
					LastName ="van Rossum",
					DoB = new DateTime(1998, 05,18),
					Gender = 0,
					EMail ="gvr@hr.nl",
					PhoneNumber ="0640643724",
					Admin = false,
					Experience = 2,
					ProfileImage = "",
					SupervisorID = 1
				},
				new EmployeeModel
				{
					ID = 9,
					Name = "Yukihiro",
					LastName = "Matsumoto",
					DoB = new DateTime(1998, 05,18),
					Gender = 0,
					EMail = "ym@hr.nl",
					PhoneNumber = "0640643724",
					Admin = false,
					Experience = 3,
					ProfileImage = "",
					SupervisorID = 1
				},
				new EmployeeModel
				{
					ID = 10,
					Name = "John",
					LastName = "Resig",
					DoB = new DateTime(1998, 05,18),
					Gender = 0,
					EMail = "jr@hr.nl",
					PhoneNumber = "0640643724",
					Admin = false,
					Experience = 3,
					ProfileImage = "",
					SupervisorID = 1
				},
				new EmployeeModel
				{
					ID = 11,
					Name = "Brian",
					LastName = "Kernighan",
					DoB = new DateTime(1998, 05,18),
					Gender = 0,
					EMail = "bk@hr.nl",
					PhoneNumber = "0640643724",
					Admin = false,
					Experience = 2,
					ProfileImage = "",
					SupervisorID = 1
				},
				new EmployeeModel
				{
					ID = 12,
					Name = "James",
					LastName = "Gosling",
					DoB = new DateTime(1998, 05,18),
					Gender = 0,
					EMail = "jg@hr.nl",
					PhoneNumber = "0640643724",
					Admin = false,
					Experience = 1,
					ProfileImage = "",
					SupervisorID = 1
				},
				new EmployeeModel
				{
					ID = 13,
					Name = "Mark",
					LastName = "Zuckerberg",
					DoB = new DateTime(1998, 05,18),
					Gender = 0,
					EMail = "mz@hr.nl",
					PhoneNumber = "0640643724",
					Admin = false,
					Experience = 1,
					ProfileImage = "",
					SupervisorID = 1
				},
				new EmployeeModel
				{
					ID = 14,
					Name = "Larry",
					LastName = "Page",
					DoB = new DateTime(1998, 05,18),
					Gender = 0,
					EMail = "lp@hr.nl",
					PhoneNumber = "0640643724",
					Admin = false,
					Experience = 1,
					ProfileImage = "",
					SupervisorID = 1
				},
				new EmployeeModel
				{
					ID = 15,
					Name = "Sergey",
					LastName = "Brin",
					DoB = new DateTime(1998, 05,18),
					Gender = 0,
					EMail = "sb@hr.nl",
					PhoneNumber = "0640643724",
					Admin = false,
					Experience = 1,
					ProfileImage = "",
					SupervisorID = 1
				},
				new EmployeeModel
				{
					ID = 16,
					Name = "Tim",
					LastName = "Berners-Lee",
					DoB = new DateTime(1998, 05,18),
					Gender = 0,
					EMail = "tbl@hr.nl",
					PhoneNumber = "0640643724",
					Admin = false,
					Experience = 4,
					ProfileImage = "",
					SupervisorID = 1
				},
				new EmployeeModel
				{
					ID = 17,
					Name = "Gijs",
					LastName = "Puelinckx",
					DoB = new DateTime(2001, 05,24),
					Gender = 0,
					EMail = "0958956@hr.nl",
					PhoneNumber = "0646889367",
					Admin = true,
					Experience = 5,
					ProfileImage = "",
				}
		};
		public static List<Availability> availabilities = new List<Availability>()
		{
			new Availability{ AvailabilityID=1, Start = new DateTime(2019, 11, 18, 8,0,0), End = new DateTime(2019, 11, 18, 10,0,0), Weekly = true },
			new Availability{ AvailabilityID=2, Start = new DateTime(2019, 11, 18, 10,0,0), End = new DateTime(2019, 11, 18, 12,0,0), Weekly = true },
			new Availability{ AvailabilityID=3, Start = new DateTime(2019, 11, 18, 12,0,0), End = new DateTime(2019, 11, 18, 14,0,0), Weekly = true},
			new Availability{ AvailabilityID=4, Start = new DateTime(2019, 11, 18, 14,0,0), End = new DateTime(2019, 11, 18, 16,0,0), Weekly = true},
			new Availability{ AvailabilityID=5, Start = new DateTime(2019, 11, 18, 16,0,0), End = new DateTime(2019, 11, 18, 18,0,0), Weekly = true},

			new Availability{ AvailabilityID=6, Start = new DateTime(2019, 11, 19, 8,0,0), End = new DateTime(2019, 11, 19, 10,0,0), Weekly = true },
			new Availability{ AvailabilityID=7, Start = new DateTime(2019, 11, 19, 10,0,0), End = new DateTime(2019, 11, 19, 12,0,0), Weekly = true },
			new Availability{ AvailabilityID=8, Start = new DateTime(2019, 11, 19, 12,0,0), End = new DateTime(2019, 11, 19, 14,0,0), Weekly = true},
			new Availability{ AvailabilityID=9, Start = new DateTime(2019, 11, 19, 14,0,0), End = new DateTime(2019, 11, 19, 16,0,0), Weekly = true},
			new Availability{ AvailabilityID=10, Start = new DateTime(2019, 11, 19, 16,0,0), End = new DateTime(2019, 11, 19, 18,0,0), Weekly = true},

			new Availability{ AvailabilityID=11, Start = new DateTime(2019, 11, 20, 8,0,0), End = new DateTime(2019, 11, 20, 10,0,0), Weekly = true },
			new Availability{ AvailabilityID=12, Start = new DateTime(2019, 11, 20, 10,0,0), End = new DateTime(2019, 11, 20, 12,0,0), Weekly = true },
			new Availability{ AvailabilityID=13, Start = new DateTime(2019, 11, 20, 12,0,0), End = new DateTime(2019, 11, 20, 14,0,0), Weekly = true},
			new Availability{ AvailabilityID=14, Start = new DateTime(2019, 11, 20, 14,0,0), End = new DateTime(2019, 11, 20, 16,0,0), Weekly = true},
			new Availability{ AvailabilityID=15, Start = new DateTime(2019, 11, 20, 16,0,0), End = new DateTime(2019, 11, 20, 18,0,0), Weekly = true},

			new Availability{ AvailabilityID=16, Start = new DateTime(2019, 11, 21, 8,0,0), End = new DateTime(2019, 11, 21, 10,0,0), Weekly = true },
			new Availability{ AvailabilityID=17, Start = new DateTime(2019, 11, 21, 10,0,0), End = new DateTime(2019, 11, 21, 12,0,0), Weekly = true },
			new Availability{AvailabilityID=18, Start = new DateTime(2019, 11, 21, 12,0,0), End = new DateTime(2019, 11, 21, 14,0,0), Weekly = true},
			new Availability{ AvailabilityID=19,Start = new DateTime(2019, 11, 21, 14,0,0), End = new DateTime(2019, 11, 21, 16,0,0), Weekly = true},
			new Availability{ AvailabilityID=20,Start = new DateTime(2019, 11, 21, 16,0,0), End = new DateTime(2019, 11, 21, 18,0,0), Weekly = true},

			new Availability{AvailabilityID=21, Start = new DateTime(2019, 11, 22, 8,0,0), End = new DateTime(2019, 11, 22, 10,0,0), Weekly = true },
			new Availability{AvailabilityID=22, Start = new DateTime(2019, 11, 22, 10,0,0), End = new DateTime(2019, 11, 22, 12,0,0), Weekly = true },
			new Availability{AvailabilityID=23, Start = new DateTime(2019, 11, 22, 12,0,0), End = new DateTime(2019, 11, 22, 14,0,0), Weekly = true},
			new Availability{AvailabilityID=24, Start = new DateTime(2019, 11, 22, 14,0,0), End = new DateTime(2019, 11, 22, 16,0,0), Weekly = true},
			new Availability{AvailabilityID=25, Start = new DateTime(2019, 11, 22, 16,0,0), End = new DateTime(2019, 11, 22, 18,0,0), Weekly = true},

			new Availability{AvailabilityID=26, Start = new DateTime(2019, 11, 23, 8,0,0), End = new DateTime(2019, 11, 23, 10,0,0), Weekly = true },
			new Availability{AvailabilityID=27, Start = new DateTime(2019, 11, 23, 10,0,0), End = new DateTime(2019, 11, 23, 12,0,0), Weekly = true },
			new Availability{AvailabilityID=28, Start = new DateTime(2019, 11, 23, 12,0,0), End = new DateTime(2019, 11, 23, 14,0,0), Weekly = true},
			new Availability{AvailabilityID=29, Start = new DateTime(2019, 11, 23, 14,0,0), End = new DateTime(2019, 11, 23, 16,0,0), Weekly = true},
			new Availability{ AvailabilityID=30,Start = new DateTime(2019, 11, 23, 16,0,0), End = new DateTime(2019, 11, 23, 18,0,0), Weekly = true},

			new Availability{AvailabilityID=31, Start = new DateTime(2019, 11, 24, 8,0,0), End = new DateTime(2019, 11, 24, 10,0,0), Weekly = true },
			new Availability{ AvailabilityID=32,Start = new DateTime(2019, 11, 24, 10,0,0), End = new DateTime(2019, 11, 24, 12,0,0), Weekly = true },
			new Availability{AvailabilityID=33, Start = new DateTime(2019, 11, 24, 12,0,0), End = new DateTime(2019, 11, 24, 14,0,0), Weekly = true},
			new Availability{AvailabilityID=34, Start = new DateTime(2019, 11, 24, 14,0,0), End = new DateTime(2019, 11, 24, 16,0,0), Weekly = true},
			new Availability{AvailabilityID=35, Start = new DateTime(2019, 11, 24, 16,0,0), End = new DateTime(2019, 11, 24, 18,0,0), Weekly = true},
		};
		public static List<EmployeeAvailability> employeeAvailabilities = SetEmployeeAvailabilities();
		public static List<EmployeeFunction> employeeFunctions = SetEmployeeFunctions();
		public static List<EmployeeSkill> employeeSkills = SetEmployeeSkills();
		public static List<EmployeeLocations> employeeLocations = SetEmployeeLocations();

		public static List<Skill> skills = new List<Skill>
		{
			new Skill{SkillID=1, Name = "Dutch"},
			new Skill{SkillID=2, Name = "English"},
			new Skill{SkillID=3, Name = "German"}
		};
		public static Location location = new Location()
		{
			LocationID = 1,
			Name = "Kralingse Zoom",
			Street = "Kralingse Zoom",
			StreetNumber = 91,
			Postalcode = "3063 ND",
			City = "Rotterdam",
			Additions = ""
		};
		public static List<Function> functions = new List<Function>()
		{
			new Function{FunctionID =1, Name = "Chef"},
			new Function{FunctionID =2, Name = "Sous Chef"},
			new Function{FunctionID =3, Name = "Linecook"},
			new Function{FunctionID =4, Name = "Bar"},
			new Function{FunctionID =5, Name = "Floor"},
			new Function{FunctionID =6, Name = "Floor Manager"},
		};

		public Shift shift = new Shift()
		{
			ShiftID = 1,
			Start = new DateTime(2020, 1, 12, 10, 0, 0),
			End = new DateTime(2020, 1, 12, 12, 0, 0),
			Location = location,
			Weekly = true,
			ShiftFunctions = SetShiftFunctions(),

		};
		public static List<ShiftSkills> shiftSkills = new List<ShiftSkills>();
		public static List<ShiftFunction> shiftFunctions = new List<ShiftFunction>();
		private static List<EmployeeAvailability> SetEmployeeAvailabilities()
		{
			int c = 1;
			foreach (EmployeeModel employee in employees)
			{
				int t = 20;
				int range = availabilities.Count() - 1;
				Availability availabilty = new Availability();
				for (int i = 0; i < t; i++)
				{
					int id = RandomID(range) + 1;
					availabilty = availabilities.FirstOrDefault(a => a.AvailabilityID == id);
					EmployeeAvailability ea = new EmployeeAvailability { EmployeeAvailabilityID=c, Employee = employee, Availability = availabilty, AvailabilityID = id, EmployeeID = employee.ID };
					employeeAvailabilities.Add(ea);
					c++;
				}
				counts.Clear();
			}
			return employeeAvailabilities;
		}
		private static List<EmployeeFunction> SetEmployeeFunctions()
		{
			int c = 1;
			foreach (EmployeeModel employee in employees)
			{
				int t = 1;
				int range = functions.Count() - 1;
				for (int i = 0; i < t; i++)
				{
					int id = RandomID(range) + 1;
					EmployeeFunction ef = new EmployeeFunction {EmployeeFunctionID = c, FunctionID = id, EmployeeID = employee.ID };
					employeeFunctions.Add(ef);
					c++;
				}
				counts.Clear();
			}
			return employeeFunctions;
		}

		private static List<EmployeeSkill> SetEmployeeSkills()
		{
			int c = 1;
			foreach (EmployeeModel employee in employees)
			{
				int t = 1;
				int range = skills.Count() - 1;
				for (int i = 0; i < t; i++)
				{
					int id = RandomID(range) + 1;
					EmployeeSkill es = new EmployeeSkill { EmployeeSkillID = c, SkillID = id, EmployeeModelID = employee.ID };
					employeeSkills.Add(es);
					c++;
				}
				counts.Clear();
			}
			return employeeSkills;
		}

		private static List<EmployeeLocations> SetEmployeeLocations()
		{
			int c = 1;
			foreach (EmployeeModel employee in employees)
			{
				EmployeeLocations el = new EmployeeLocations { EmployeeLocationsID = c, LocationID = 1, EmployeeModelID = employee.ID };
				employeeLocations.Add(el);
				c++;
			}
			return employeeLocations;
		}

		private static ICollection<ShiftSkills> SetShiftSkills()
		{
			List<ShiftSkills> shiftSkills = new List<ShiftSkills>();
			int c = 0;
			foreach (Skill sk in skills)
			{
				ShiftSkills ss = new ShiftSkills
				{
					ShiftskillsID = c,
					Skill = sk,
					SkillID = sk.SkillID,
					ShiftID = 1,
				};
				shiftSkills.Add(ss);
				c++;
			}
			return shiftSkills;
		}

		private static ICollection<ShiftFunction> SetShiftFunctions()
		{
			List<ShiftFunction> shiftFunctions = new List<ShiftFunction>();
			int c = 1;
			foreach (Function f in functions)
			{
				ShiftFunction sf = new ShiftFunction
				{
					ShiftFunctionID = c,
					Function = f,
					FunctionID = f.FunctionID,
					ShiftID = 1
				};
				shiftFunctions.Add(sf);
			c++;
			}
			return shiftFunctions;
		}
		public Shift ReturnShift()
		{
			return shift;
		}

		public static List<int> counts = new List<int>();
		public static int RandomID(int range)
		{
			Random rnd = new Random();
			if (range <= 0)
				return 1;

			int cnt = rnd.Next(range);
			if (counts != null)
			{
				foreach (int c in counts)
				{
					if (cnt == c)
					{
						return RandomID(range);
					}
				}
				counts.Add(cnt);
			}
			return cnt;
		}



		public void GeneratorFilterFunction()
		{
			foreach (ShiftFunction func in shift.ShiftFunctions)
			{

				//Iedereen die kan werken.
				List<EmployeeModel> available = (from row in availabilities
												 join ea in employeeAvailabilities on row.AvailabilityID equals ea.AvailabilityID
												 join employee in employees on ea.EmployeeID equals employee.ID
												 where row.Weekly ?
												 // if availability is weekly
												 row.Start.DayOfWeek == shift.Start.DayOfWeek && row.End.DayOfWeek == shift.End.DayOfWeek &&
												 (shift.Start.TimeOfDay >= row.Start.TimeOfDay && shift.End.TimeOfDay <= row.End.TimeOfDay) :
												 // if not
												 (shift.Start >= row.Start && shift.End <= row.End)
												 select employee).ToList();


				available = available.Distinct(new EmployeeComparer()).ToList();
				List<EmployeeModel> function = (from employee in available
												join ef in employeeFunctions on employee.ID equals ef.EmployeeID
												where ef.FunctionID == func.FunctionID
												select employee).ToList();

				//Iedereen die kan werken, juiste functie heeft en locatie.
				List<EmployeeModel> location = (from employee in function
												join el in employeeLocations on employee.ID equals el.EmployeeModelID
												where el.LocationID == shift.LocationID
												select employee).ToList();

				//Iedereen die kan werken, juiste functie en locatie heeft en skill.
				List<EmployeeModel> skill = (from employee in location
											 join es in employeeSkills on employee.ID equals es.EmployeeModelID
											 join sk in skills on es.SkillID equals sk.SkillID
											 join ss in shiftSkills on sk.SkillID equals ss.SkillID
											 where shift.ShiftSkills.Contains(ss)
											 select employee).ToList();

				resultAfterAvailabilityFilter = available;
				resultAfterFunctionFilter = function;
				resultAfterLocationFilter = location;
				resultAfterSkillFilter = skill;
			}
		}

		public List<EmployeeModel> GetAllEmployees(){return employees;}
		public List<EmployeeModel> GetEmployeesWithCorrectAvailabilities() { return resultAfterAvailabilityFilter; }
		public List<EmployeeModel> GetEmployeesWithCorrectFunctions() { return resultAfterFunctionFilter; }
		public List<EmployeeModel> GetEmployeesWithCorrectSkills() { return resultAfterSkillFilter; }
		public List<EmployeeModel> GetEmployeesWithCorrectLocation() { return resultAfterLocationFilter; }

		class EmployeeComparer : IEqualityComparer<EmployeeModel>
	{
		public bool Equals(EmployeeModel x, EmployeeModel y) => x.ID == y.ID;
		public int GetHashCode(EmployeeModel obj) => obj.GetHashCode();
	}
	}
}
