function SummaryCard({ title, value, color }) {
  return (
    <div className="bg-white rounded-xl shadow-md p-6 border">

      <div
        className={`w-12 h-12 rounded-lg flex items-center justify-center ${color}`}
      >
      </div>

      <h2 className="mt-4 text-3xl font-bold">
        {value}
      </h2>

      <p className="text-gray-500">
        {title}
      </p>

    </div>
  );
}

export default SummaryCard;