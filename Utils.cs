using Gride.Data;
using Gride.Models;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GrideTest
{
	public static class Utils
	{
		public static void InitDB(ApplicationDbContext context)
		{
			DbInitializer.Initialize(context);

			AddShift(new Shift()
			{
				Start = DateTime.Now,
				End = DateTime.Now.AddDays(14),
				LocationID = 1,
				Location = context.Locations.First(x => x.LocationID == 1),
			}, new int[] { 1 }, new int[] { 2 }, new int[] { 0, 5 }, context);
			AddShift(new Shift()
			{
				Start = DateTime.Now.AddDays(1),
				End = DateTime.Now.AddDays(15),
				LocationID = 1,
				Location = context.Locations.First(x => x.LocationID == 1),
				Weekly = true,
			}, new int[] { 1 }, new int[] { 2 }, new int[] { 0, 5 }, context);

			AddWork(new Work()
			{
				ShiftID = 1,
				EmployeeID = 1,
				Overtime = 0,
				Delay = 0
			}, context);

		}

		public static void ReInitDB(ApplicationDbContext context)
		{
			Type dbType = context.GetType();
			MethodInfo toList = typeof(Enumerable).GetMethod("ToList");
			MethodInfo clear = typeof(List<>).GetMethod("Clear");
			// clear every DbSet in the database
			foreach (PropertyInfo prop in dbType.GetProperties())
				if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
				{
					clear.Invoke(toList.Invoke(prop, null), null);
					context.SaveChanges();
				}

			InitDB(context);
		}

		private async static void AddWork(Work work, ApplicationDbContext _context)
		{
			_context.Add(work);
			await _context.SaveChangesAsync();
		}

		private async static void AddShift(Shift shift, int[] selectedSkills, int[] selectedFunctions, int[] selectedFunctionsMax, ApplicationDbContext _context)
		{
			if (selectedSkills != null)
			{
				shift.ShiftSkills = new List<ShiftSkills>();
				foreach (var skill in selectedSkills)
				{
					var skillToAdd = new ShiftSkills
					{
						ShiftID = shift.ShiftID,
						Shift = shift,
						SkillID = skill,
						Skill = _context.Skill.Single(s => s.SkillID == skill)
					};
					shift.ShiftSkills.Add(skillToAdd);
				}
			}
			if (selectedFunctions != null)
			{
				shift.ShiftFunctions = new List<ShiftFunction>();
				foreach (var function in selectedFunctions)
				{
					int functionID = function;
					int maxCnt = functionID - 1;
					var functionToAdd = new ShiftFunction
					{
						ShiftID = shift.ShiftID,
						Shift = shift,
						FunctionID = functionID,
						Function = _context.Function.Single(f => f.FunctionID == function),
						MaxEmployees = selectedFunctionsMax[maxCnt]
					};
					shift.ShiftFunctions.Add(functionToAdd);
				}
			}
			_context.Add(shift);
			await _context.SaveChangesAsync();
			if (shift.Weekly)
			{
				ICollection<Shift> children = CreateChildren(shift, _context);
				foreach (Shift child in children)
				{
					_context.Add(child);
					child.ShiftFunctions = new List<ShiftFunction>();
					foreach (ShiftFunction sf in shift.ShiftFunctions)
					{
						ShiftFunction shiftFunction = new ShiftFunction
						{
							Function = sf.Function,
							FunctionID = sf.FunctionID,
							MaxEmployees = sf.MaxEmployees,
							ShiftID = child.ShiftID
						};
						child.ShiftFunctions.Add(shiftFunction);
					}
					child.ShiftSkills = new List<ShiftSkills>();
					foreach (ShiftSkills ss in shift.ShiftSkills)
					{
						ShiftSkills shiftSkills = new ShiftSkills
						{
							Skill = ss.Skill,
							SkillID = ss.SkillID,
							ShiftID = child.ShiftID
						};
						child.ShiftSkills.Add(shiftSkills);
					}
				}
				await _context.SaveChangesAsync();
				shift.ShiftChildren = children;
			}
			await _context.SaveChangesAsync();
		}


		private static ICollection<Shift> CreateChildren(Shift shift, ApplicationDbContext _context)
		{
			Shift tmpShift = new Shift();
			tmpShift.Start = shift.Start;
			tmpShift.End = shift.End;
			tmpShift.Weekly = true;
			tmpShift.Location = shift.Location;
			tmpShift.LocationID = shift.LocationID;
			tmpShift.ParentShiftID = shift.ShiftID;
			ICollection<Shift> children = new List<Shift>();
			for (int i = 1; i < 52; i++)
			{
				Shift child = new Shift();
				child.Weekly = true;
				child.Location = tmpShift.Location;
				child.LocationID = tmpShift.LocationID;
				child.ParentShiftID = tmpShift.ParentShiftID;
				child.Start = shift.Start.AddDays(7 * i);
				child.End = shift.End.AddDays(7 * i);
				children.Add(child);
			}
			return children;
		}
	}
}