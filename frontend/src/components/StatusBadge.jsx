function StatusBadge({ status }) {

    let className =
        "px-3 py-1 rounded-full text-sm font-semibold";

    if (status === "Completed") {

        className +=
            " bg-green-100 text-green-700";

    }

    else if (status === "Completed With Errors") {

        className +=
            " bg-yellow-100 text-yellow-700";

    }

    else {

        className +=
            " bg-red-100 text-red-700";

    }

    return (

        <span className={className}>
            {status}
        </span>

    );

}

export default StatusBadge;