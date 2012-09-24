<html>
<head>
<title>File Versions</title>
</head>

<body>
<table border="1">
<tr><th>SimChartMedicalOffice Files</th><th>Last Modified Date</th></tr>
<%
dim fs,fo,f
set fs=Server.CreateObject("Scripting.FileSystemObject")
set fo=fs.GetFolder("C:\inetpub\wwwroot\SimOffice\bin")

For Each f in fo.Files
	If f.Type = "Application extension" then
		If Left(f.Name, 21) = "SimChartMedicalOffice" then
			Response.Write("<tr><td>")
			Response.Write(f.Name)
			Response.Write("</td><td>")
			Response.Write(f.DateLastModified)
			Response.Write("</td></tr>")
		End If
	End If
Next

set f=nothing
set fo=nothing
set fs=nothing
%>
</table>
</body>

</html>