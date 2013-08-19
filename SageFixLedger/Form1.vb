Imports System.IO
Imports System.Data
Imports System.Data.SqlClient


Public Class frmMain

    Private Sub cmdIniciar_Click(sender As Object, e As EventArgs) Handles cmdIniciar.Click

        Dim numeroRegistos As Integer = 0
        Dim qry, qry2 As String
        Dim reader3 As SqlDataReader
        Dim contagem As Integer = 0
        Dim totalrestante As Double = 0

        Dim connString As String = "Server=" + txtServidor.Text + ";Database=" + txtBaseDados.Text + ";User Id=" + txtUsername.Text + ";Password=" + txtPassword.Text + ";MultipleActiveResultSets=True;"

        Dim conn As New SqlConnection(connString)
        Dim conn2 As New SqlConnection(connString)

        Try
            conn.Open()

            Dim cmd As New SqlCommand("select * from AccountTransactionDetails;", conn)
            Dim reader As SqlDataReader = cmd.ExecuteReader

            While (reader.Read) 'obter numero de resultados
                numeroRegistos += 1
            End While

            reader.Close()

            pb1.Maximum = numeroRegistos 'calibrar a barra de progresso

            'voltar a correr a query
            Dim cmd2 As New SqlCommand("select docserial,docid,docnumber,partyid,totalpayedamount from AccountTransactionDetails;", conn)
            Dim reader2 As SqlDataReader = cmd2.ExecuteReader

            pb1.Value = 0

            Dim cmd3, cmd4 As New SqlCommand
            cmd3.Connection = conn
            cmd4.Connection = conn

            While (reader2.Read) 'update dos documentos da customerledgeraccount
                qry = "select totalpendingamount from customerledgeraccount where totalpendingamount > 0 and transserial = '" + reader2.GetString(0) + "' and transdocument = '" + reader2.GetString(1) + "' and transdocnumber = " + reader2.GetInt64(2).ToString + " and partyid = " + reader2.GetInt64(3).ToString
                cmd3.CommandText = qry
                reader3 = cmd3.ExecuteReader()
                contagem = 0
                While (reader3.Read) 'correr os resultados do customerledger account
                    totalrestante = reader3.GetDouble(0)
                    totalrestante -= reader2.GetDouble(4)

                    qry2 = "update customerledgeraccount set totalpendingamount = " + totalrestante.ToString.Replace(",", ".") + " where transserial = '" + reader2.GetString(0) + "' and transdocument = '" + reader2.GetString(1) + "' and transdocnumber = " + reader2.GetInt64(2).ToString + " and partyid = " + reader2.GetInt64(3).ToString
                    cmd4.CommandText = qry2
                    cmd4.ExecuteNonQuery()
                End While
                reader3.Close()
                pb1.Value += 1
            End While
            reader2.Close()
            MsgBox("Concluido com sucesso!", vbOKOnly, "Terminado") 'mensagem de concluido
        Catch ex As Exception
            MsgBox(qry2 + vbNewLine + connString + vbNewLine + vbNewLine + ex.ToString)
            End
        End Try
    End Sub
End Class
