using MiniLojaVirtual.Domain.Models.Abstracts.Base;

namespace MiniLojaVirtual.Domain.Models.Abstracts;

public abstract class Entity : IEntity
{
	public long Id { get; set; }
	public Guid Code { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }
	public bool IsDeleted { get; set; }

	public void MarkAsDeleted()
	{
		UpdatedAt = DateTime.Now;
		IsDeleted = true;
	}
}