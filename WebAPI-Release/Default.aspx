<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>

    <div>
        <table width="1019px">
            <tr>
                <th width="67px">API</th>
                <th width="520px">URL</th>
                <th width="346px">参数</th>
                <th>执行</th>
            </tr>
        </table>
        <form action="API.aspx">
            <table height="100px" width="1019px">
                <tr>
                    <td width="67px">Login</td>
                    <td width="520px">http://gcsj.hxlxz.com/API.aspx?api=login</td>
                    <td width="346px">
                        <input type="hidden" name="API" value="login" />
                        username<input name="username" value="root" /><br />
                        password<input name="password" value="root123456" />
                    </td>
                    <td>
                        <input type="submit" value="执行" />
                    </td>
                </tr>
            </table>
        </form>
        <form action="API.aspx">
            <table height="130px" width="1019px">
                <tr>
                    <td width="67px">Lookup</td>
                    <td width="520px">http://gcsj.hxlxz.com/API.aspx?api=lookup</td>
                    <td width="346px">
                        <input type="hidden" name="API" value="lookup" />
                        name<input name="name" value="T" /><br />
                        starttime<input name="starttime" value="2016-12-26 00:00:00" /><br />
                        endtime<input name="endtime" value="2016-12-30 00:00:00" />
                    </td>
                    <td>
                        <input type="submit" value="执行" />
                    </td>
                </tr>
            </table>
        </form>
        <form action="API.aspx">
            <table height="80px" width="1019px">
                <tr>
                    <td width="67px">Delete</td>
                    <td width="520px">http://gcsj.hxlxz.com/API.aspx?api=delete</td>
                    <td width="346px">
                        <input type="hidden" name="API" value="delete" />
                        id<input name="id" />
                    </td>
                    <td>
                        <input type="submit" value="执行" />
                    </td>
                </tr>
            </table>
        </form>
        <form action="API.aspx">
            <table height="100px" width="1019px">
                <tr>
                    <td width="67px">Upload</td>
                    <td width="520px">http://gcsj.hxlxz.com/API.aspx?api=upload</td>
                    <td width="346px">
                        <input type="hidden" name="API" value="upload" />
                        name<input name="name" value="T" /><br />
                        value<input name="value" value="23.5" />
                    </td>
                    <td>
                        <input type="submit" value="执行" />
                    </td>
                </tr>
            </table>
        </form>
    </div>
</body>
</html>
