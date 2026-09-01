using AutoMapper;
using Marketplace.Domain.Entities;
using Marketplace.Application.DTOs.Products;
using Marketplace.Application.DTOs.Categories;
using Marketplace.Application.DTOs.Orders;
using Marketplace.Application.DTOs.Reviews;
using Marketplace.Application.DTOs.ShoppingCart;
using Marketplace.Application.DTOs.Users;
using Marketplace.Domain.Entities.Enums;
namespace Marketplace.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>()
                 .ForMember(dest => dest.FinalPrice, opt => opt.MapFrom(src => src.DiscountedPrice ?? src.Price))
                 .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
                 .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src => src.Seller != null ? src.Seller.FullName : string.Empty));


            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Views, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.SoldQuantity, opt => opt.MapFrom(src => 0));


            CreateMap<UpdateProductDto, Product>();

            CreateMap<ProductImage, ProductImageDto>();

            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ParentCategoryName,
                                   opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : string.Empty))
                .ForMember(dest => dest.ProductCount,
                                   opt => opt.MapFrom(src => src.Products.Count(p => p.IsActive)));


            CreateMap<CreateCategoryDto, Category>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));


            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : string.Empty))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.Status , opt => opt.MapFrom(src => src.Status.ToString()));
      


            CreateMap<CreateOrderDto, Order>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => OrderStatus.Pending))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => PaymentStatus.Pending));


            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.ProductImageUrl, opt => 
                opt.MapFrom(src => src.Product != null  && src.Product.Images.Any() ? src.Product.Images.First(i => i.IsPrimary).ImageUrl : null));


            CreateMap<CreateOrderItemDto, OrderItem>();


            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty));
                

            CreateMap<ReviewDto, Review>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => false));


            CreateMap<User, UserDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Any() ? src.Products.Count(p => p.IsActive) : 0))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews.Any() ? src.Reviews.Count(p => p.IsApproved) : 0));

            

            CreateMap<CartItem, CartItemDto>()
               .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
               .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : 0))
               .ForMember(dest => dest.ProductFinalPrice, opt => opt.MapFrom(src => src.Product != null ? (src.Product.DiscountedPrice ?? src.Product.Price) : 0))
               .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Quantity * (src.Product != null ? (src.Product.DiscountedPrice ?? src.Product.Price) : 0)))
               .ForMember(dest => dest.MaxStock, opt => opt.MapFrom(src => src.Product != null ? src.Product.StockQuantity : 0))
               .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product != null && src.Product.Images.Any() ? src.Product.Images.First(i => i.IsPrimary).ImageUrl : null));


            CreateMap<ShoppingCart, ShoppingCartDto>();

            CreateMap<ShoppingCartDto, ShoppingCart>();




        }
    }
}
