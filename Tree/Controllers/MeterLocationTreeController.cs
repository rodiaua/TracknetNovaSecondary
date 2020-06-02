using Microsoft.AspNetCore.Mvc;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tree.Models;
using Tree.Services;

namespace Tree.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class MeterLocationTreeController
  {
    private readonly MeterLocationTreeService _meterLocationTreeService;
    public MeterLocationTreeController(MeterLocationTreeService meterLocationTreeService)
    {
      _meterLocationTreeService = meterLocationTreeService;
    }

    [HttpGet("[action]")]
    public IEnumerable<MeterLocationTree> GetAll()
    {
      return _meterLocationTreeService.GetAll();
    }

    [HttpPost("[action]")]

    public async Task DeleteAll()
    {
      await _meterLocationTreeService.DeleteAll();
    }

    [HttpPost("[action]")]
    public async Task CreateElement([FromBody] MeterLocationTree meterLocationTree)
    {
      if (meterLocationTree != null)
      {
        await _meterLocationTreeService.AddLocationElement(meterLocationTree);
      }
    }

    [HttpPost("[action]/{id}")]
    public async Task DeleteNode(int id)
    {
      if (id != 0) { await _meterLocationTreeService.DeleteNode(id); }
    }

    [HttpPut("[action]/{childId}/{parentId?}")]
    public async Task UpdateNode(int? parentId, int childId, [FromBody]string locationElement)
    {
      await _meterLocationTreeService.UpdateNode(parentId, new MeterLocationTree() { Id = childId, LocationElement = locationElement}) ;
    }

    [HttpGet("[action]/{id}")]
    public IEnumerable<MeterLocationTree> GetAllChildElemnets(int id)
    {
      return _meterLocationTreeService.GetAllChildElemnets(id);
    }

    [HttpGet("[action]/{id}")]
    public IEnumerable<MeterLocationTree> GetFirstCicleChildElemnets(int id)
    {
      return _meterLocationTreeService.GetFirstCicleChildElemnets(id);
    }

    [HttpPost("[action]/{count}")]
    public async Task<string> CreateNode(int count)
    {
      return await _meterLocationTreeService.CreateNode(count);
    }

    [HttpPost("[action]")]
    public IEnumerable<MeterLocationTree> Search([FromBody]string substring)
    {
      if (!string.IsNullOrEmpty(substring))
      {
        return _meterLocationTreeService.SearchBySubstring(substring);
      }
      return null;
    }

  }
}
