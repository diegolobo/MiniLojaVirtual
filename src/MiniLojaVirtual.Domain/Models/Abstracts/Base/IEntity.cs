namespace MiniLojaVirtual.Domain.Models.Abstracts.Base;

public interface IEntity
{
	public long Id { get; set; }
	public Guid Code { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public bool IsDeleted { get; set; }
}