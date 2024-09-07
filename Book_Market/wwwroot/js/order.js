var dataTable;
$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    }
    else {
        if (url.includes("pending")) {
            loadDataTable("pending");
        }
        else {
            if (url.includes("completed")) {
                loadDataTable("completed");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                }
                else {
                    loadDataTable("all");
                }
            }
        }
    }
    
});

function loadDataTable(status) {
    dataTable = $('#myTable').DataTable({
        "ajax": { url: '/admin/order/getall?status=' + status },
        "columns": [
            { data: "id", "width": "15%" },
            { data: "name", "width": "25%" },
            { data: "phoneNumber", "width": "15%" },
            { data: "applicationUser.email", "width": "20%" },
            { data: "orderStatus", "width": "15%" },
            { data: "orderTotal", "width": "15%" },
            {
                data: "id",
                "render": function (data) {
                    return ` <div class="btn-group" role="group">
                                <a href="/admin/order/details/${data}" class="btn btn-primary" style="width:100px">
                                    <i class="bi bi-pen"></i> 
                                </a>
                            </div>`;
                },"width":"10%"
            }
        ]
    });
}


