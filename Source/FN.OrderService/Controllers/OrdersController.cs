﻿using FN.Application.Systems.Orders;
using FN.ViewModel.Systems.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FN.OrderService.Controllers
{
    [Route("api/order")]
    [ApiController, Authorize]
    public class OrdersController : BasesController
    {
        private readonly IOrderService _service;
        public OrdersController(IOrderService service)
        {
            _service = service;
        }
        [HttpGet("hello")]
        public IActionResult Hello()
        {
            return Ok("Hello from Email Service");
        }
        [HttpPost("request-order")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _service.CreateOrder(userId.Value, request);
            return Ok(result);
        }
        [HttpPost("create-url")]
        public IActionResult PaymentRequest([FromBody] PaymentInformationModel model)
        {
            var result = _service.CreatePaymentUrl(model, HttpContext);
            return Ok(result);
        }
        [HttpGet("payment-callback")]
        public IActionResult PaymentCallback()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var response = _service.PaymentExecute(Request.Query, userId.Value);
            return Accepted(response);
        }
    }
}
