$(document).ready(function () {
    $('table').DataTable({
        responsive: true,
        "columnDefs": [{
            "targets": 'nosort',
            "orderable": false
        }]
    });
})