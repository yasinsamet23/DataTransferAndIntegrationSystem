import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import api from "../services/api";
import SummaryCard from "../components/SummaryCard";
import StatusBadge from "../components/StatusBadge";
import Toast from "../components/Toast";


function Dashboard() {
  const [users, setUsers] = useState([]);
  const [transferLogs, setTransferLogs] = useState([]);
  const [isTransferring, setIsTransferring] = useState(false);
  const [transferStatus, setTransferStatus] = useState("");
  const [showUploadModal, setShowUploadModal] = useState(false);
  const [selectedFile, setSelectedFile] = useState(null);
  const [isUploading, setIsUploading] = useState(false);
  const [toastData, setToastData] = useState(null);

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

  useEffect(() => {

    if (!toastData)
      return;

    const timer = setTimeout(() => {

      setToastData(null);

    }, 4000);

    return () => clearTimeout(timer);

  }, [toastData]);


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

      setIsTransferring(true);

      setTransferStatus("Transferring...");

      const response = await api.post("/transfer/start");

      loadUsers();

      loadTransferLogs();

      setTransferStatus(response.data.message);
      setToastData(response.data);

    }

    catch (error) {

      console.error(error);

      setTransferStatus("Transfer failed.");

      setToastData({
        totalRecords: 0,
        successfulRecords: 0,
        failedRecords: 0,
        message: "Transfer failed."
      });

    }

    finally {

      setIsTransferring(false);

    }

  };

  const uploadCsv = async () => {


    if (!selectedFile)
      return;

    try {
      setIsUploading(true);
      const formData = new FormData();

      formData.append("file", selectedFile);
      const response = await api.post(
        "/transfer/upload",
        formData,
        {
          headers: {
            "Content-Type": "multipart/form-data"
          }
        }
      );

      setTransferStatus(response.data.message);
      setToastData(response.data);

      await loadUsers();
      await loadTransferLogs();

      setSelectedFile(null);
      setShowUploadModal(false);




    }
    catch (error) {

      console.error(error);

      setTransferStatus("CSV upload failed.");

      setToastData({
        totalRecords: 0,
        successfulRecords: 0,
        failedRecords: 0,
        message: "CSV upload failed."
      });

    }
    finally {

      setIsUploading(false);

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
            Choose a transfer method to import users into the system.
          </p>
        </div>

        <div className="flex flex-col items-end">

          <div className="flex gap-3">

            <button
              onClick={() => setShowUploadModal(true)}
              className="bg-green-600 text-white px-6 py-3 rounded-lg hover:bg-green-700 transition duration-200 cursor-pointer"
            >
              Upload CSV
            </button>

            <button
              onClick={startTransfer}
              disabled={isTransferring}
              className="bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition duration-200 cursor-pointer disabled:bg-gray-400"
            >
              {
                isTransferring
                  ? "Transferring..."
                  : "External API Transfer"
              }
            </button>

          </div>

          {
            transferStatus && (

              <p className="mt-2 text-sm text-gray-600">

                {transferStatus}

              </p>

            )
          }

        </div>

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
              <th className="text-left p-4">Transfer Id</th>
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
                    <td className="p-4" title={log.id}>
                      {log.id.substring(0, 8)}
                    </td>

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
      {
        showUploadModal && (
          <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">

            <div className="bg-white rounded-xl shadow-xl w-[500px] p-6">

              <h2 className="text-2xl font-bold mb-2">
                Upload CSV
              </h2>

              <p className="text-gray-500 mb-6">
                Select a CSV file to import users.
              </p>

              <input
                type="file"
                accept=".csv"
                className="w-full
        border
        rounded-lg
        p-2
        cursor-pointer
        transition
        duration-200
        hover:border-blue-500
        focus:outline-none
        focus:ring-2
        focus:ring-blue-500"
                onChange={(e) =>
                  setSelectedFile(e.target.files[0])
                }
              />

              <div className="flex justify-end gap-3 mt-6">

                <button
                  onClick={() => {
                    setShowUploadModal(false);
                    setSelectedFile(null);
                  }}
                  className="
        px-5
        py-2
        rounded-lg
        bg-white
        border
        border-gray-300
        hover:bg-gray-100
        transition
        duration-200
        cursor-pointer
    "
                >
                  Cancel
                </button>

                <button
                  onClick={uploadCsv}
                  disabled={!selectedFile || isUploading}
                  className="
        px-5
        py-2
        rounded-lg
        bg-green-600
        text-white
        hover:bg-green-700
        transition
        duration-200
        disabled:bg-gray-400
        disabled:cursor-not-allowed
        cursor-pointer
    "
                >
                  {isUploading ? "Uploading..." : "Upload"}
                </button>

              </div>

            </div>

          </div>
        )
      }

      {toastData && (

        <Toast

          result={toastData}

        />

      )}
    </div>
  );
}


export default Dashboard;