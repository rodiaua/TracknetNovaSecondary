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

    /// <summary>
    ///Retreives all nodes ordered by id (https://{address}:{port}/api/meterlocationtree/getall/).
    /// </summary>
    [HttpGet("[action]")]
    public IEnumerable<MeterLocationTree> GetAll()
    {
      return _meterLocationTreeService.GetAll();
    }

    /// <summary>
    ///Deletes all nodes(https://{address}:{port}/api/meterlocationtree/deleteall/).
    /// </summary>
    [HttpPost("[action]")]
    public async Task DeleteAll()
    {
      await _meterLocationTreeService.DeleteAll();
    }

    /// <summary>
    ///Creates a node(https://{address}:{port}/api/meterlocationtree/createnode/).
    ///{	
    ///"LocationElement":"",
    ///"Path": []
    ///}
    ///LocationElement - string, name of the node
    ///Path - int[], contains parent path, leave array empty if it's root node.
    /// </summary>
    [HttpPost("[action]")]
    public async Task CreateNode([FromBody] MeterLocationTree meterLocationTree)
    {
      if (meterLocationTree != null)
      {
        await _meterLocationTreeService.AddLocationElement(meterLocationTree);
      }
    }

    /// <summary>
    ///Delete a node and its children(https://localhost:44368/api/meterlocationtree/deletenode/{id})
    /// </summary>
    [HttpPost("[action]/{id}")]
    public async Task DeleteNode(int id)
    {
      if (id != 0) { await _meterLocationTreeService.DeleteNode(id); }
    }

    /// <summary>
    ///Upadtes(https://localhost:44368/api/meterlocationtree/updatenode/{nodeId}/{parenNodeId})
    ///
    /// </summary>
    [HttpPut("[action]/{childId}/{parentId?}")]
    public async Task UpdateNode(int? parentId, int childId, [FromBody]string locationElement)
    {
      await _meterLocationTreeService.UpdateNode(parentId, new MeterLocationTree() { Id = childId, LocationElement = locationElement });
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
