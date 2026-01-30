interface Props {
  index: number;
  value: number;
  max: number;
}

export default function ProgressBar({ index, value, max }: Props) {
  function getColour() {
    switch (index) {
      case 1:
        return "bg-green-500";
      case 2:
        return "bg-yellow-300";
      case 3:
        return "bg-yellow-500";
      case 4:
        return "bg-orange-500";
      case 5:
        return "bg-red-600";
      default:
        return "bg-gray-400";
    }
  }

  const percentage = () => (value / max) * 100;

  return (
    <div className="w-full max-w-md h-6 rounded-full bg-gray-200 shadow-inner relative overflow-hidden">
      <div
        className={`h-full ${getColour()} transition-all duration-500 ease-out`}
        style={{ width: `${Math.min(percentage(), 100)}%` }}
      />
      <div className="absolute inset-0 flex items-center justify-center">
        <span className="text-sm font-medium text-gray-800">{value}</span>
      </div>
    </div>
  );
}
