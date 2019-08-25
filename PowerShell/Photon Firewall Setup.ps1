$fw = New-Object -ComObject hnetcfg.fwpolicy2
$ruleTcp = New-Object -ComObject HNetCfg.FWRule
$ruleUdp = New-Object -ComObject HNetCfg.FWRule

$ruleTcp.Name = "Photon SocketServer TCP"
$ruleTcp.Protocol = 6 #NET_FW_IP_PROTOCOL_TCP
$ruleTcp.LocalPorts = "80,443,843,943,4520-4522,4530-4533,4540,4541,6060-6063,9090-9093,19090-19093"
$ruleTcp.Enabled = $true

$ruleTcp.Profiles = 7 #all
$ruleTcp.Action = 1 # NET_FW_ACTION_ALLOW

$ruleUdp.Name = "Photon SocketServer UDP"
$ruleUdp.Protocol = 17 #NET_FW_IP_PROTOCOL_Udp
$ruleUdp.LocalPorts = "5055-5058,40001,27000-27003"
$ruleUdp.Enabled = $true

$ruleUdp.Profiles = 7 #all
$ruleUdp.Action = 1 # NET_FW_ACTION_ALLOW

$fw.Rules.Add($ruleTcp)
$fw.Rules.Add($ruleUdp)