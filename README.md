# Desafio C# Web API

## Descrição
Criar uma Web API em C# com o banco de dados fornecido neste repositório, e desenvolver os seguintes end-points:

•	Obter a média de preço dos produtos das notas fiscais de uma empresa, arrecadadas em intervalo de datas “de-até” no formato (dd/MM/aaaa). O resultado deve ser agrupado por mês

•	Cadastrar nota fiscal com a seguinte regra de validação no servidor:<br />
    o	O valor total de uma nota fiscal deve ser a soma do preço dos seus produtos cadastrados na tabela NotasFiscaisProdutos

•	Atualizar uma nota fiscal com a seguinte regra de validação no servidor:<br />
    o	Somente é permitido atualizar a nota fiscal caso esta seja a última nota enviada pela empresa
    
•	Deletar uma nota fiscal

## Instruções e Avaliação

•   Para que avaliemos seu desafio, faça um fork deste repositório e execute um pull-request.

•   Descrever neste README do projeto os exemplos de uso da sua API

## Pontos avaliados:<br />
•	Consultas ao Banco de Dados <br />
•	Estrutura da Web API<br />
•	Qualidade do código<br />

Candidato: Jeremias dos Santos Francelino<br />
Exemplos de uso: <br />
<pre>
• Obter a média de preço dos produtos das notas fiscais de uma empresa, arrecadadas em intervalo de datas “de-até” no formato       (dd/MM/aaaa). O resultado deve ser agrupado por mês:<br />
    URL: apimarqdesafio/NotasFiscais/?100=idEmpresa&101=dd/MM/aaaa&102=dd/MM/aaaa
        100 -> IdEmpresa
        101 -> Data inicial
        102 -> Data final
        Todos os campos de filtro são obrigatórios
    Método: GET
    Retorno:
        [
            {
                'Mes': string,
                'Produtos': [
                    {
                        'Nome': string,
                        'Marca': string,
                        'MediaPreco': decimal
                    },...
                ]
            },...
        ]
• Cadastrar nota fiscal com a seguinte regra de validação no servidor:
o O valor total de uma nota fiscal deve ser a soma do preço dos seus produtos cadastrados na tabela NotasFiscaisProdutos
    URL: apimarqdesafio/NotasFiscais
        Enviar no body objeto (application/json) a ser cadastrado no seguinte formato:
            {
                'IdEmpresa': int,
                'DataHora': string (MM/dd/aaaa HH:mm:ss),
                'Total': decimal,
                'Produtos': [
                    {
                        'Id': int, 
                        'Quantidade': int, 
                        'Preco': decimal
                    },...
                ]
            }
    Método: POST
• Atualizar uma nota fiscal com a seguinte regra de validação no servidor:
o Somente é permitido atualizar a nota fiscal caso esta seja a última nota enviada pela empresa
    URL: apimarqdesafio/NotasFiscais
        Enviar no body objeto (application/json) a ser atualizado no seguinte formato:
            {
                'Id': int,
                'IdEmpresa': int,
                'Total': decimal,
                'DataHora': string (MM/dd/aaaa HH:mm:ss)
            }
    Método: PUT
• Deletar uma nota fiscal
    URL: apimarqdesafio/NotasFiscais/IdNotaFiscal
    Método: DELETE
<pre/>
