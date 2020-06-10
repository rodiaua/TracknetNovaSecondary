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
    public async Task<IEnumerable<MeterLocationTree>> GetAll()
    {
      return await _meterLocationTreeService.GetAll();
    }

    /// <summary>
    ///Retreives all nodes which do not have parent nodes ordered by id (https://{address}:{port}/api/meterlocationtree/getallrootnodes/).
    /// </summary>
    [HttpGet("[action]")]
    public async Task<IEnumerable<MeterLocationTree>> GetAllRootNodes()
    {
      return await _meterLocationTreeService.GetAllRootNodes();
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
    ///Delete a node and its children(https://{address}:{port}/api/meterlocationtree/deletenode/{id})
    /// </summary>
    [HttpPost("[action]/{id}")]
    public async Task DeleteNode(int id)
    {
      if (id != 0) { await _meterLocationTreeService.DeleteNode(id); }
    }

    /// <summary>
    ///Upadtes a node(https://{address}:{port}/api/meterlocationtree/updatenode/{nodeId}/{parenNodeId})
    ///Body: "{name}"
    ///name - optional string value that changes a node name.
    ///parenNodeId - optional int parameter of the parent node that is the node where a child node is moving to while updating.
    ///(Set parenNodeId = 0 if you wnat to make updated node root node.)
    ///When a node is moving to another node its children moving along with it.
    ///THROWS an EXSEPTION if parent node is moving to its child node.
    /// </summary>
    [HttpPut("[action]/{childId}/{parentId?}")]
    public async Task UpdateNode(int? parentId, int childId, [FromBody]string locationElement)
    {
      await _meterLocationTreeService.UpdateNode(parentId, new MeterLocationTree() { Id = childId, LocationElement = locationElement });
    }

    /// <summary>
    ///Retreives all child nodes (https://{address}:{port}/api/meterlocationtree/getallchildelemnets/{nodeId}).
    /// </summary>
    [HttpGet("[action]/{id}")]
    public IEnumerable<MeterLocationTree> GetAllChildNodes(int id)
    {
      return _meterLocationTreeService.GetAllChildElemnets(id);
    }

    /// <summary>
    ///Retreives closest child nodes (https://{address}:{port}/api/meterlocationtree/getfirstciclechildelemnets/{nodeId}).
    /// </summary>
    [HttpGet("[action]/{id}")]
    public IEnumerable<MeterLocationTree> GetFirstCicleChildNodes(int id)
    {
      return _meterLocationTreeService.GetFirstCicleChildElemnets(id);
    }

    /// <summary>
    ///Performs a search by substring and retrieves the nodes, firstly those which starts with the substring then those which contains the substring
    ///(https://{address}:{port}/api/meterlocationtree/search/).
    ///Body: "{substring}"
    /// </summary>
    [HttpPost("[action]")]
    public async Task<IEnumerable<MeterLocationTree>> Search([FromBody]string substring)
    {
      if (!string.IsNullOrEmpty(substring))
      {
        return await _meterLocationTreeService.SearchBySubstring(substring);
      }
      return null;
    }


    //Create recordes for tests (https://{address}:{port}/api/meterlocationtree/createnode/{count}).
    //count - amount of city
    //each city containt 2 streets
    //each street contains 2 houses
    //each houses containt 2 meters
    [HttpPost("[action]/{count}")]
    public async Task<string> CreateNode(int count)
    {
      return await _meterLocationTreeService.CreateNode(count);
    }

    
  }
}
