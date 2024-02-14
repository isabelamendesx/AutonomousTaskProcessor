using AutoMapper;
using AutonomousTaskProcessor.Entities;
using AutonomousTaskProcessor.Services;
using Microsoft.AspNetCore.Mvc;
using TaskProcessorAPI.Dtos;

namespace TaskProcessorAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ProcessController : ControllerBase
{
    private readonly IProcessExecutor _processExecutor;
    private readonly IProcessManager _processManager;
    private readonly IMapper _mapper;

    public ProcessController(IProcessManager processManager, IMapper mapper, IProcessExecutor processExecutor)
    {
        _processManager = processManager;
        _mapper = mapper;
        _processExecutor = processExecutor;
    }

    [HttpGet]
    public IActionResult Get([FromQuery] int skip = 0, [FromQuery] int take = 20, [FromQuery] bool isActive = true)
    {
        var processes = isActive ? _processManager.ListActiveProcesses().Result : _processManager.ListInactiveProcesses().Result;

        return Ok(processes
            .Skip(skip)
            .Take(take)
            .Select(process => _mapper.Map<ReadProcessesDto>(process)));
    }

    [HttpGet("/{id}")]
    public IActionResult GetProcessById(int id)
    {
        var process = _processManager.Check(id).GetAwaiter().GetResult();

        if (process == null) return NotFound("Process not found");

        return Ok(_mapper.Map<ReadProcessDto>(process));
    }

    [HttpPost]
    public IActionResult Post()
    {
        _processManager.Create();

        var newProcess = _processManager.ListActiveProcesses().Result.MaxBy(process => process.Id);

        return CreatedAtAction(nameof(GetProcessById), new { id = newProcess!.Id }, newProcess);
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartProcessing()
    {
        await _processExecutor.Start();
        return Ok();
    }

    [HttpPost("close")]
    public async Task<IActionResult> CloseProcessing()
    {
        await _processExecutor.Close();
        return Ok();
    }

    [HttpPost("restart")]
    public async Task<IActionResult> RestartProcesses()
    {
        await _processExecutor.Restart();
        return Ok();
    }

    [HttpPut("cancel/{id}")]
    public async Task<IActionResult> CancelProcess(int id)
    {
        var process = _processManager.Check(id).GetAwaiter().GetResult();

        if (process == null) return NotFound("Process not found");

        if (process.Status.Equals(StatusProcess.InProgress))
        {
            await _processExecutor.CancelProcess(process.Id);
        }
        else
        {
            await _processManager.Cancel(process.Id);
        }

        return NoContent(); ;
    }



}
