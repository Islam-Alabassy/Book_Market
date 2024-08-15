var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#myTable').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            { data: "title", "width": "15%" },
            { data: "isbn", "width": "15%" },
            { data: "author", "width": "15%" },
            { data: "listPrice", "width": "5%" },
            { data: "category.name", "width": "15%" },
            {
                data: "productId",
                "render": function (data) {
                    return ` <div class="btn-group" role="group">
                                <a href="/admin/product/edit/${data}" class="btn btn-primary" style="width:100px">
                                    <i class="bi bi-pen"></i> Edit
                                </a>
                            </div>
                            <div class="btn-group" role="group">
                                <a onClick=Delete('/admin/product/deleteapi/${data}') class="btn btn-danger col-12" style="width:100px">
                                    <i class="bi bi-trash"></i> Delete
                                </a>
                            </div>`;
                },"width":"20%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
            
        }
    });
}
