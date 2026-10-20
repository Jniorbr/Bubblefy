using Bubblefy.Business;
using Bubblefy.Data;
using Bubblefy.Model;
using Microsoft.EntityFrameworkCore;

namespace Bubblefy.Service;

public class UsuarioService(AppDbContext context)
{
    public Usuario Cadastrar(string nome, string senha)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Length > 50 || string.IsNullOrWhiteSpace(senha))
            throw new ArgumentException("Informe um usuario (ate 50 caracteres) e uma senha.");

        if (context.Usuarios.Any(u => u.Nome == nome))
            throw new InvalidOperationException("Esse nome de usuario ja existe.");

        string salt = Senha.GerarSalt();
        Usuario usuario = new() { Nome = nome, Salt = salt, SenhaHash = Senha.GerarHash(senha, salt) };
        context.Usuarios.Add(usuario);
        context.SaveChanges();
        return usuario;
    }

    public Usuario? Autenticar(string nome, string senha)
    {
        Usuario? usuario = context.Usuarios.AsNoTracking().FirstOrDefault(u => u.Nome == nome);

        if (usuario == null)
            return null;

        return Senha.Verificar(senha, usuario.Salt, usuario.SenhaHash) ? usuario : null;
    }
}