import { useEffect, useState } from "react";
import api from "../services/api";

function Users() {

  const [users, setUsers] = useState([]);

  useEffect(() => {

    loadUsers();

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

  return (
    <div>

      <h1 className="text-3xl font-bold mb-2">
        Users
      </h1>

      <p className="text-gray-500 mb-8">
        All transferred users.
      </p>

      <div className="bg-white rounded-xl shadow-md border overflow-hidden">

        <table className="w-full">

          <thead className="bg-gray-100">

            <tr>

              <th className="text-left p-4">
                Name
              </th>

              <th className="text-left p-4">
                Email
              </th>

              <th className="text-left p-4">
                Phone
              </th>

              <th className="text-left p-4">
                Created Date
              </th>

            </tr>

          </thead>

          <tbody>

            {
              users.map(user => (

                <tr
                  key={user.id}
                  className="border-t hover:bg-gray-50"
                >

                  <td className="p-4">
                    {user.name}
                  </td>

                  <td className="p-4">
                    {user.email}
                  </td>

                  <td className="p-4">
                    {user.phone}
                  </td>

                  <td className="p-4">
                    {
                      new Date(user.createdDate)
                        .toLocaleString()
                    }
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

export default Users;