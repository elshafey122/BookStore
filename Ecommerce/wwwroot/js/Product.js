var DataTable;
$(document).ready(function () {
    LoadDataTable();
});

function LoadDataTable() {
    DataTable= $('#tblData').DataTable({
        "ajax": { url: '/admin/product/getall' },
        "columns": [
            { data: 'title',"width":"15%" },
            { data: 'isbn', "width": "15%" },
            { data: 'price', "width": "15%" },
            { data: 'author', "width": "15%" },
            { data: 'category.name', "width": "15%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="btn-group">
                             <a href="/admin/product/upsert?ProductId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit </a>
                             <a onClick=Delete('/admin/product/remove/${data}') class="btn btn-danger mx-2" > <i class="bi bi-trash3-fill"></i>   Delete </a>
                           <div/>`
                },
                "width": "25%"
            }
        ]
    });
}

function Delete(Url) {
    console.log(Url);
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
                url: Url,
                type: 'DELETE',
                success: function (data) {
                    DataTable.ajax.reload();
                    console.log(data.messag);
                    toastr.success(data.message);
                }
            });
        }
    });
}