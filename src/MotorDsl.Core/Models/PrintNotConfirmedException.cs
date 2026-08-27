namespace MotorDsl.Core.Models;

/// <summary>
/// El documento se envio completo, pero la impresora <b>no confirmo que lo haya impreso</b>.
/// <para>
/// Que el <c>Write</c> vuelva sin error solo significa que el stack Bluetooth acepto los bytes.
/// Si la impresora se queda sin energia, se traba o se apaga con el documento ya en su buffer, el
/// envio termina "bien" y el ticket sale por la mitad o no sale: el operador se va creyendo que
/// imprimio. El timeout de escritura no cubre este caso, porque no llega a bloquearse ninguna
/// escritura.
/// </para>
/// <para>
/// Lo unico que lo detecta es preguntar algo <b>encolado</b> despues del documento y esperar la
/// respuesta: llega recien cuando la impresora proceso todo lo anterior, y en una termica procesar
/// es imprimir. Medido en un moto g42: la escritura tarda ~47 ms y la respuesta a <c>GS I 67</c>
/// llega a los 4 167 ms (MTP-II) y 3 540 ms (58HB6), que es lo que tardan en imprimir.
/// </para>
/// <para>
/// Se clasifica como <see cref="PrintErrorType.Protocol"/>, que el handler por defecto <b>NO</b>
/// reintenta: el documento pudo haberse impreso parcialmente y reimprimir solo, sin que nadie mire
/// el papel, gastaria rollo y podria duplicar un comprobante. La decision de reimprimir es del
/// operador.
/// </para>
/// </summary>
public class PrintNotConfirmedException : Exception
{
    public PrintNotConfirmedException(string message) : base(message) { }
    public PrintNotConfirmedException(string message, Exception inner) : base(message, inner) { }
}
