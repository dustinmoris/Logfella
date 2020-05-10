# Run requests in parallel:

for ($i = 1; $i -le 10; $i++) {
    $i
    Start-Job { Invoke-WebRequest -Uri "http://localhost:5000/ping?cid=PING-$using:i" }
    Start-Job { Invoke-WebRequest -Uri "http://localhost:5000/pong?cid=PONG-$using:i" }
    Start-Job { Invoke-WebRequest -Uri "http://localhost:5000/foo?cid=FOO-$using:i" }
}

# Wait for all jobs to complete

While (Get-Job -State "Running") { Start-Sleep 2 }
Remove-Job *