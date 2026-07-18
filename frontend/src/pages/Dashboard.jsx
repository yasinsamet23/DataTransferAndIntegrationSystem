import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import api from "../services/api";
import SummaryCard from "../components/SummaryCard";
import StatusBadge from "../components/StatusBadge";


function Dashboard() {
  const [users, setUsers] = useState([]);
  const [transferLogs, setTransferLogs] = useState([]);

  const totalSuccessful = transferLogs.reduce(
    (total, log) => total + log.successCount,
    0
  );

  const totalFailed = transferLogs.reduce(
    (total, log) => total + (log.totalRecords - log.successCount),
    0
  );

  const totalRecords = transferLogs.reduce(
    (total, log) => total + log.totalRecords,
    0
  );

  const errorRate =
    totalRecords > 0
      ? ((totalFailed / totalRecords) * 100).toFixed(1)
      : 0;

  useEffect(() => {

    loadUsers();

    loadTransferLogs();

  }, []);


  const loadUsers = async () => {

    try {

      const response = await api.get("/users");

      setUsers(response.data);

    }

    catch (error) {

      console.error(error);

    }

  };

  const loadTransferLogs = async () => {

    try {

      const response = await api.get("/logs");

      setTransferLogs(response.data);

    }

    catch (error) {

      console.error(error);

    }

  };

  const startTransfer = async () => {

    try {

      const response = await api.post("/transfer/start");

      alert(response.data.message);

      loadUsers();

      loadTransferLogs();

    }

    catch (error) {

      console.error(error);

      alert("Transfer failed.");

    }
  };

  return (
    <div>
      <h1 className="text-3xl font-bold mb-2">
        Dashboard
      </h1>

      <p className="text-gray-500 mb-8">
        Monitor transfers, records and system health.
      </p>

      {/* Summary Cards */}
      <div className="grid grid-cols-2 gap-6">
        <SummaryCard
          title="Total Users"
          value={users.length}
          color="bg-blue-100"
        />

        <SummaryCard
          title="Successful Transfers"
          value={totalSuccessful}
          color="bg-green-100"
        />

        <SummaryCard
          title="Failed Records"
          value={totalFailed}
          color="bg-red-100"
        />

        <SummaryCard
          title="Error Rate"
          value={`${errorRate}%`}
          color="bg-yellow-100"
        />
      </div>

      {/* Start Transfer */}
      <div className="mt-8 bg-white rounded-xl shadow-md p-6 flex justify-between items-center">
        <div>
          <h2 className="text-xl font-bold">
            Ready to Transfer
          </h2>

          <p className="text-gray-500">
            Synchronize data from external API.
          </p>
        </div>

        <button
          onClick={startTransfer}
          className="bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition duration-200 cursor-pointer"
        >
          Start Transfer
        </button>
      </div>

      {/* Recent Transfer Logs */}
      <div className="mt-8 bg-white rounded-xl shadow-md border">
        <div className="flex justify-between items-center p-6 border-b">
          <div>
            <h2 className="text-xl font-bold">
              Recent Transfer Logs
            </h2>

            <p className="text-gray-500">
              Latest data sync operations across systems.
            </p>
          </div>

          <Link
            to="/transfer-logs"
            className="text-blue-600 font-semibold hover:underline cursor-pointer"
          >
            View All
          </Link>
        </div>

        <table className="w-full">
          <thead className="bg-gray-50">
            <tr>
              <th className="text-left p-4">Transfer Date</th>
              <th className="text-left p-4">Total Records</th>
              <th className="text-left p-4">Success Count</th>
              <th className="text-left p-4">Status</th>
            </tr>
          </thead>
          <tbody>
            {
              transferLogs
                .slice(0, 5)
                .map((log) => (

                  <tr
                    key={log.id}
                    className="border-t"
                  >

                    <td className="p-4">
                      {
                        new Date(log.transferDate)
                          .toLocaleString()
                      }
                    </td>

                    <td className="p-4">
                      {log.totalRecords}
                    </td>

                    <td className="p-4">
                      {log.successCount}
                    </td>

                    <td className="p-4">

                      <StatusBadge
                        status={log.status}
                      />

                    </td>

                  </tr>

                ))
            }
          </tbody>


        </table>
      </div>
    </div>
  );
}


export default Dashboard;