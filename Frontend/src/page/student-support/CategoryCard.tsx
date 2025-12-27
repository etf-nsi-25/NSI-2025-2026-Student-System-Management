import "./CategoryCard.css";

type CategoryCardProps = {
  title: string;
  description: string;
  onClick?: () => void;
};

export default function CategoryCard({ title, description, onClick }: CategoryCardProps) {
  const isClickable = typeof onClick === "function";

  return (
    <button
      type="button"
      className={`cc-card ${isClickable ? "cc-clickable" : ""}`}
      onClick={onClick}
      disabled={!isClickable}
    >
      <div className="cc-title">{title}</div>
      <div className="cc-desc">{description}</div>
    </button>
  );
}