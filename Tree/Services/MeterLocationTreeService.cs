using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tree.Data;
using Tree.Models;
using Tree.Services;

namespace Tree.Services
{
  public class MeterLocationTreeService
  {
    private readonly ApplicationDbContext _dbContext;

    public MeterLocationTreeService(ApplicationDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public IEnumerable<MeterLocationTree> GetAll()
    {
      return _dbContext.MeterLocationTree.OrderBy(m => m.Id).ToList();
    }

    public async Task DeleteAll()
    {
      _dbContext.MeterLocationTree.RemoveRange(GetAll());
      await _dbContext.SaveChangesAsync();
    }

    public async Task AddLocationElement(MeterLocationTree meterLocationTree)
    {
      if (meterLocationTree.Path == null)
      {
        meterLocationTree.Path = new int[0];
      }
      _dbContext.MeterLocationTree.Add(meterLocationTree);
      _dbContext.SaveChanges();
      var path = meterLocationTree.Path.ToList();
      path.Add(meterLocationTree.Id);
      meterLocationTree.Path = path.ToArray();
      _dbContext.MeterLocationTree.Update(meterLocationTree);
      await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteNode(int id)
    {
      MeterLocationTree meterLocationTree = _dbContext.MeterLocationTree.Find(id);
      var trees = _dbContext.MeterLocationTree.AsEnumerable().Where(p => p.Path.Compare(meterLocationTree.Path)).ToList();
      _dbContext.MeterLocationTree.RemoveRange(trees);
      await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateNode(int? parentId, MeterLocationTree meterLocationTree)
    {
      if (_dbContext.MeterLocationTree.Any(p => p.Id == meterLocationTree.Id))
      {
        var childNode = _dbContext.MeterLocationTree.Find(meterLocationTree.Id);
        if (parentId != null)
        {
          var parentNode = _dbContext.MeterLocationTree.Find(parentId);
          if (parentNode != null)
          {
            var path = parentNode.Path.ToList();
            path.Add(meterLocationTree.Id);
            var childList = _dbContext.MeterLocationTree.AsEnumerable().Where(p => p.Path.IsChildOf(childNode.Path));
            if (childList != null)
            {
              foreach (var child in childList)
              {
                var pathForChild = new List<int>();
                pathForChild.AddRange(path);
                pathForChild.Add(meterLocationTree.Id);
                Array.Copy(child.Path, childNode.Path.Length - 1, pathForChild.ToArray(), path.ToArray().Length, child.Path.Length - childNode.Path.Length - 1);
                child.Path = (int[])pathForChild.ToArray().Clone();
                _dbContext.MeterLocationTree.Update(child);
              }
            }
            childNode.Path = path.ToArray();
          }
        }
        if (!childNode.LocationElement.Equals(meterLocationTree.LocationElement) && !string.IsNullOrEmpty(meterLocationTree.LocationElement))
        {
          childNode.LocationElement = meterLocationTree.LocationElement;
        }
        _dbContext.MeterLocationTree.Update(childNode);
        await _dbContext.SaveChangesAsync();
      }
    }

    public IEnumerable<MeterLocationTree> GetAllChildElemnets(int parentId)
    {
      var parentElemnet = _dbContext.MeterLocationTree.Find(parentId);
      return _dbContext.MeterLocationTree.AsEnumerable().Where(p => p.Path.IsChildOf(parentElemnet.Path)).OrderBy(p=>p.Path.Length).ToList();
    }

    public IEnumerable<MeterLocationTree> GetFirstCicleChildElemnets(int parentId)
    {
      var parentElemnet = _dbContext.MeterLocationTree.Find(parentId);
      return _dbContext.MeterLocationTree.AsEnumerable().Where(p => p.Path.IsFirstCicleChildOf(parentElemnet.Path)).ToList();
    }

    public async Task<string> CreateNode(int count)
    {
      TimeSpan startTime = new TimeSpan(DateTime.Now.Ticks);

      //int[] cityPath = new int [0];
      int[] streetPath = new int[0];
      int[] housePath = new int[0];
      int[] meterPath = new int[0];
      for (int i = 1; i <= count; i++)
      {
        MeterLocationTree city = new MeterLocationTree();
        city.LocationElement = $"Місто{i}";
        city.Path = new int[0];
        await AddLocationElement(city);
        Array.Resize(ref streetPath, city.Path.Length);
        Array.Copy(city.Path, streetPath, city.Path.Length);
        for (int k = 1; k <= 2; k++)
        {
          MeterLocationTree street = new MeterLocationTree();
          street.LocationElement = $"Вулиця{i}{k}";
          Array.Copy(streetPath, street.Path = new int[streetPath.Length], streetPath.Length);
          await AddLocationElement(street);
          Array.Resize(ref housePath, street.Path.Length);
          Array.Copy(street.Path, housePath, street.Path.Length);
          for (int t = 1; t <= 2; t++)
          {
            MeterLocationTree house = new MeterLocationTree();
            house.LocationElement = $"Дім{i}{k}{t}";
            Array.Copy(housePath, house.Path = new int[housePath.Length], housePath.Length);
            await AddLocationElement(house);
            Array.Resize(ref meterPath, house.Path.Length);
            Array.Copy(house.Path, meterPath, house.Path.Length);
            for (int z = 1; z <= 2; z++)
            {
              MeterLocationTree meter = new MeterLocationTree();
              meter.LocationElement = $"Лічильник{i}{k}{t}{z}";
              Array.Copy(meterPath, meter.Path = new int[meterPath.Length], meterPath.Length);
              await AddLocationElement(meter);
            }
          }
        }
      }
      await _dbContext.SaveChangesAsync();
      TimeSpan finishTime = new TimeSpan(DateTime.Now.Ticks);
      return finishTime.Subtract(startTime).ToString();
    }

    public IEnumerable<MeterLocationTree> SearchBySubstring(string substring)
    {
      var elementThatStarts = _dbContext.MeterLocationTree.Where(p => p.LocationElement.ToLower().StartsWith(substring.ToLower())).ToList();
      var elementThatContains = _dbContext.MeterLocationTree.Where(p => p.LocationElement.ToLower().Contains(substring.ToLower()) && !p.LocationElement.ToLower().StartsWith(substring.ToLower())).ToList();
      elementThatStarts.AddRange(elementThatContains);
      return elementThatStarts;
    }
  }

  static class Extensions
  {
    //The method compare two arays and return true if element of the second array equals the elements of first array in the same order
    //and the length of the first  array is >= then length of second array.
    //So we can detect appropriate tree node and its children
    public static bool Compare(this int[] child, int[] parent)
    {
      if (child.Length >= parent.Length)
      {
        int n = 0;
        for (int i = 0; i < parent.Length; i++)
        {
          if (child[i] == parent[i]) n++;
        }
        if (n == parent.Length) return true;
      }
      return false;
    }

    //The method compare two arays and return true if element of the second array equals the elements of first array in the same order
    //and the length of the first array is > then length of second array.
    //So we can detect children those are relevant to the tree node
    public static bool IsChildOf(this int[] child, int[] parent)
    {
      if (child.Length > parent.Length)
      {
        int n = 0;
        for (int i = 0; i < parent.Length; i++)
        {
          if (child[i] == parent[i]) n++;
        }
        if (n == parent.Length) return true;
      }
      return false;
    }

    //The method compare two arays and return true if element of the second array equals the elements of first array in the same order
    //and the length-1 of the first array is = to length of second array.
    //So we can detect first cicle children those are relevant to the tree node
    public static bool IsFirstCicleChildOf(this int[] child, int[] parent)
    {
      if (child.Length - 1 == parent.Length)
      {
        int n = 0;
        for (int i = 0; i < parent.Length; i++)
        {
          if (child[i] == parent[i]) n++;
        }
        if (n == parent.Length) return true;
      }
      return false;
    }
  }
}
