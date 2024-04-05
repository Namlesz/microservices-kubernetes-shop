using System.ComponentModel.DataAnnotations;

namespace Product.Api.Requests;

internal sealed record DeleteProductRequest([Required] string Code);