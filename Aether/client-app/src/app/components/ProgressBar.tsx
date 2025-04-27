interface Props {
  index: number;
  value: number;
}

export default function ProgressBar({ index, value }: Props) {
  function getColour() {
    switch (index) {
      case 1:
        return "bg-green-500";
      case 2:
        return "bg-yellow-100";
      case 3:
        return "bg-yellow-500";
      case 4:
        return "bg-red-100";
      case 5:
        return "bg-red-500";
    }
  }

  return (
    <div className="h-16 w-100 rounded-lg border border-gray-500 bg-gray-200 relative flex items-center justify-center">
      <p className="text-xl font-semibold text-gray-700 z-10">{value}</p>
      <div
        className={`h-16 ${getColour()} rounded-lg absolute top-0 left-0`}
        style={{ width: `${value}px` }}
      ></div>
      {/* {[2, 3, 4, 5].map((i) => (
        <div
          key={i}
          className="absolute top-0 bottom-0 w-px bg-gray-500"
          style={{ left: `${(i - 1) * 20}%` }}
        >
          <p className="">50</p>
        </div>
      ))} */}
    </div>
  );
}
