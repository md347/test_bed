function QueryPort ([string]$ip, [string]$port) {
    if (($socket = New-Object Net.Sockets.TcpClient).BeginConnect($ip, $port, $null, $null).AsyncWaitHandle.WaitOne(500)) {
        $stream = $socket.GetStream();
        Start-Sleep -m 1000; $text = '';
        while ($stream.DataAvailable) {
            $text += [char]$stream.ReadByte()
        };
        
        if ($text.Length -eq 0) {
            $text = "No Banner Given"
        };
        
        "TCP:22 is open : $text";
        $socket.Close()
    }
}
